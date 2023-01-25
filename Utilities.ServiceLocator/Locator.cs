using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml.Linq;
using Utilities.ServiceLocator.Interfaces;

namespace Utilities.ServiceLocator
{
    public class Locator : ILocator
    {
        public ILogger _logger;
        public Locator(IServiceProvider provider, ILogger<Locator> logger)
        {
            _provider = provider;
            _logger = logger;
        }
        private static object _locker = new object();
        private static List<Type> _assemblyTypes = null;
        private static ConcurrentDictionary<string, object> _entries = new ConcurrentDictionary<string, object>();
        private readonly IServiceProvider _provider;
        private static List<string> GetSolutionAssemblies()
        {
            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                                .Select(x => AssemblyName.GetAssemblyName(x).FullName);
            return assemblies.ToList();
        }
        private static List<string> GetDomainAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                .Select(x => x.FullName);
            return assemblies.ToList();
        }
        //private static Assembly[] GetReferencedAssemblies()
        //{
        //    var assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
        //    return assemblies.ToArray();
        //}
        public static List<string> GetReferencedAssemblies()
        {
            var returnAssemblies = new List<Assembly>();
            var loadedAssemblies = new HashSet<string>();
            var assembliesToCheck = new Queue<Assembly>();

            assembliesToCheck.Enqueue(Assembly.GetEntryAssembly());

            while (assembliesToCheck.Any())
            {
                var assemblyToCheck = assembliesToCheck.Dequeue();

                foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
                {
                    if (!loadedAssemblies.Contains(reference.FullName))
                    {
                        try
                        {
                            var assembly = Assembly.Load(reference);
                            assembliesToCheck.Enqueue(assembly);
                            loadedAssemblies.Add(reference.FullName);
                            returnAssemblies.Add(assembly);
                        } catch
                        {

                        }
                    }
                }
            }

            return returnAssemblies.Select(x => x.FullName).ToList();
        }

        private List<Type> AssemblyTypes
        {
            get
            {
                if (_assemblyTypes == null)
                {
                    var names = GetSolutionAssemblies().AsQueryable();
                    names = names.Union(GetDomainAssemblies());
                    names = names.Union(GetReferencedAssemblies());
                    names = names.Distinct().Where(x => !x.StartsWith("Microsoft.") && !x.StartsWith("System."));


                    var assemblies = new List<Assembly>();

                    foreach(var name in names)
                    {
                        _logger.LogDebug("Assembly [" + name + "]");
                        try
                        {
                            var ass = Assembly.Load(name);
                            assemblies.Add(ass);
                        }
                        catch 
                        {

                        }
                    }



                    //Assembly.Load(
                    //var assemblies = GetReferencedAssemblies()
                    //    .Where(x => !x.FullName.StartsWith("Microsoft.") && !x.FullName.StartsWith("System.")).ToList();

                    _assemblyTypes = new List<Type>();
                    foreach (var ass in assemblies)
                    {
                        try
                        {
                            //_logger.LogDebug("Assembly [" + ass.FullName + "]");
                            var types = ass.GetTypes();

                            //_logger.LogDebug("Assembly [" + ass.FullName + "] Types [" + types.Count() + "]");

                            //foreach (var tp in types)
                            //{
                            //    _logger.LogDebug("Assembly [" + ass.FullName + "] Type [" + tp.FullName + "]");
                            //    foreach (var i in tp.GetInterfaces())
                            //    {
                            //        _logger.LogDebug("Assembly [" + ass.FullName + "] Type [" + tp.FullName + "] Interface [" + i.FullName + "]");
                            //        if (i is IService)
                            //        {
                            //            _logger.LogDebug("Assembly [" + ass.FullName + "] Service Type [" + i.FullName + "]");
                            //        }
                            //    }
                            //}
                            _assemblyTypes.AddRange(types);



                        }
                        catch //(Exception ex2)
                        {
                            //_logger.LogError(ex2, ex2.Message);
                            //var i = 0;
                        }
                    }

                    _logger.LogDebug("Total Types [" + _assemblyTypes.Count() + "]");
                }
                _logger.LogDebug("AssemblyTypes returning");
                return _assemblyTypes;
            }
        }

        public TT FindService<TT>(Func<TT, bool> whereClause)
             where TT : class, IService
        {
            try
            {
                var list = GetServicesList<TT>();
                if (whereClause != null)
                {
                    list = (from se in list
                                    where whereClause(se)
                                    select se).ToList();
                }

                var selected = list.FirstOrDefault();
                if (selected != null)
                {
                    selected = (TT)ActivatorUtilities.GetServiceOrCreateInstance(_provider, selected.GetType());
                }

                return selected;
            }
            catch (Exception ex)
            {
                var ttype = typeof(TT);
                _logger.LogDebug("Error FindService with where <TT>=[" + ttype.ToString() + "]");
                _logger.LogError(ex, "Error FindService with where <TT>=[" + ttype.ToString() + "]");
            }
            return default(TT);
        }
        public TT FindService<TT>(string name) 
            where TT : class, IService
        {
            try
            {

                var list = GetServicesList<TT>();

                var selected = (from se in list
                                where se.Name.ToUpper().Trim() == name.ToUpper().Trim()
                                select se).FirstOrDefault();

                if (selected != null)
                {
                    selected = (TT)ActivatorUtilities.GetServiceOrCreateInstance(_provider, selected.GetType());
                }

                return selected;
            } catch (Exception ex)
            {
                var ttype = typeof(TT);
                _logger.LogDebug("Error FindService name=[" + name.ToString() + "] <TT>=[" + ttype.ToString() + "]");
                _logger.LogError(ex, "Error FindService name=[" + name.ToString() + "] <TT>=[" + ttype.ToString() + "]");
            }
            return default(TT);
        }

        public void ExecutePerService<TT>(Action<TT> actionClause, Func<TT, int> orderBy = null) 
            where TT : class, IService
        {

            var list = GetServicesList<TT>();
            if (orderBy != null)
            {
                list = list.OrderBy(orderBy).ToList();
            }

            if (actionClause != null)
            {
                list.ForEach(actionClause);
            }


        }

        public TT GetServiceInstance<TT>(Type type) where TT : class
        {
            try
            {
                var i = (TT)ActivatorUtilities.GetServiceOrCreateInstance(_provider, type);
                return i;
            } catch (Exception ex)
            {
                var ttype = typeof(TT);
                _logger.LogDebug("Error GetServiceInstance Type=[" + type.ToString() + "] <TT>=[" + ttype.ToString() + "]");
                _logger.LogError(ex, "Error GetServiceInstance Type=[" + type.ToString() + "] <TT>=[" + ttype.ToString() + "]");
                return default(TT);
            }
        }

        [Obsolete("Do not use this, instead use the FindService call", true)]
        public List<TT> GetServices<TT>() where TT : class
        {
            return GetServicesList<TT>();
        }
        private List<TT> GetServicesList<TT>() where TT : class
        {
            var type = typeof(TT);
            var nm = "GetServices_" + type.Name;

            lock (_locker)
            {

                if (!_entries.ContainsKey(nm))
                {

                    _logger.LogDebug("GetServices - Loading Assembly Types <TT>=[" + type.ToString() + "]");
                    var assTypes = AssemblyTypes;

                    _logger.LogDebug("GetServices - Getting types where <TT>=[" + type.ToString() + "] count=" + assTypes.Count);

                    var tpList = (from t in assTypes
                                  where t.GetInterfaces().Contains(type)
                                  select t).ToList();

                    //_logger.LogDebug("All type for interface [" + type.FullName + "]");
                    //foreach (var tp in tpList)
                    //{
                    //    _logger.LogDebug("Type [" + tp.FullName + "]");
                    //}

                    _logger.LogDebug("GetServices - Make sure not interface and not abstract <TT>=[" + type.ToString() + "] count=" + tpList.Count);
                    var tpListFin = (from t in tpList where !t.IsInterface && !t.IsAbstract select t).ToList();
                    //_logger.LogDebug("All Creatable Types for interface [" + type.FullName + "]");
                    //foreach (var tp in tpList)
                    //{
                    //    _logger.LogDebug("Type [" + tp.FullName + "]");
                    //}

                    _logger.LogDebug("GetServices - Getting Service Instance <TT>=[" + type.ToString() + "] count=" + tpListFin.Count);
                    var cList = (from t in tpListFin select GetServiceInstance<TT>(t)).ToList();

                    _logger.LogDebug("GetServices - Adding to Dictionary <TT>=[" + type.ToString() + "] count=" + cList.Count);
                    _entries.TryAdd(nm, cList);
                }

                List<TT> list = _entries[nm] as List<TT>;


                list = (from i in list where i != null select i).ToList();

                //var cList = (from t in AssemblyTypes
                //             where t.GetInterfaces().Contains(type) && !t.IsInterface && !t.IsAbstract
                //             select GetServiceInstance<TT>(t)).ToList();
                return list;
            }
        }
    }
}
