
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.TimedTasks.Repos;
using Microsoft.Extensions.DependencyInjection;
using Utilities.TimedTasks.Configuration;
using System.IO;
using System.Reflection;

namespace Utilities.TimedTasks
{
    public static class Core
    {

        //private static ILogger _logger;
        private static bool IsInTaskCheck { get; set; }

       // private static List<ITask> taskList = new List<ITask>();

        private static object assemLoadLock = new object();

        private static List<Type> _assemblyTypes { get; set; }


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
                        }
                        catch
                        {

                        }
                    }
                }
            }

            return returnAssemblies.Select(x => x.FullName).ToList();
        }

        internal static List<Type> AssemblyTypes
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

                    foreach (var name in names)
                    {
                        //_logger.LogDebug("Assembly [" + name + "]");
                        try
                        {
                            var ass = Assembly.Load(name);
                            assemblies.Add(ass);
                        }
                        catch
                        {

                        }
                    }

                    _assemblyTypes = new List<Type>();
                    foreach (var ass in assemblies)
                    {
                        try
                        {
                            var types = ass.GetTypes();

                            _assemblyTypes.AddRange(types);
                        }
                        catch //(Exception ex2)
                        {
                            //_logger.LogError(ex2, ex2.Message);
                            //var i = 0;
                        }
                    }

                    //_logger.LogDebug("Total Types [" + _assemblyTypes.Count() + "]");
                }
                //_logger.LogDebug("AssemblyTypes returning");
                return _assemblyTypes;
            }
        }




        internal static List<ITask> GetTasks(Func<Type, ITask> creator = null)
        {
            //(ITask)DependencyResolver.Current.GetService(t)
            if (creator == null)
            {
                creator = (Type tp) => (ITask)tp.Assembly.CreateInstance(tp.FullName);
            }
            var taskList = new List<ITask>();
            if (!taskList.Any())
            {
                try
                {
                    var assTypes = AssemblyTypes;

                    var tpList = (from t in assTypes
                                  where t.GetInterfaces().Contains(typeof(ITask))
                                  select t).ToList();

                    //_logger.LogDebug("GetServices - Make sure not interface and not abstract <TT>=[" + type.ToString() + "] count=" + tpList.Count);
                    var tpListFin = (from t in tpList where !t.IsInterface && !t.IsAbstract select t).ToList();




                    var lst = (from t in tpListFin select creator(t)).ToList();


                    //var lst = (from t in types
                    //            where t.GetInterfaces().Contains(typeof(ITask))
                    //            select creator(t)).ToList();
                    taskList.AddRange(lst);
                }
                catch //(Exception ex2)
                {
                    //_logger.LogError(ex2, ex2.Message);
                    //var i = 0;
                }
            }
            return taskList;
        }
        //private static TaskServices _instance;
        // public static CacheSystem Instance => Configurator.Instance;
        public static TaskServices Instance => Configurator.Instance;
        
        public static void AddTask(ITask task)
        {
            Instance.AddTask(task);
        }


        public static void TriggerTasks()
        {
            Instance.TriggerTasks();
        }

    }
}
