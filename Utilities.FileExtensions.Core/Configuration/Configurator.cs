using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities.FileExtensions.Configuration;
using Utilities.FileExtensions.Services;

namespace Utilities.FileExtensions.AspNetCore.Configuration
{
    public static class Configurator
    {



        public static IServiceCollection AddFileServerExtensions(this IServiceCollection services, ILogger logger, Action<FileServerProviderOptions> configFunc = null)
        {
            services.AddFileServerProvider(logger, configFunc);
            services.AddScoped<IServerServices, ServerFileServices>();
            services.InitilizeFileExtensions();
            services.AddScoped(typeof(IEmbeddedFileHandling<>), typeof(EmbeddedFileHandler<>));


            return services;
        }

        /// <summary>
        /// Custom extension to handle configuration of custom FileServerProvider. Must be used with <see cref="UseFileServerProvider(IApplicationBuilder, IFileServerProvider)"/> in Configure in Startup.cs!
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static IServiceCollection AddFileServerProvider(this IServiceCollection services, ILogger logger, Action<FileServerProviderOptions> configFunc = null)
        {
            //Add our IFileServerProvider implementation as a singleton
            services.AddSingleton<IPhysicalFileServerProvider>(new PhysicalFileServerProvider(GetFileServerOptions(logger, configFunc)));
            services.AddSingleton<IEmbeddedFileServerProvider>(new EmbeddedFileServerProvider(GetEmbeddedFileOptions(logger)));


            return services;
        }


        //Get all file server configuration options
       private static  List<FileServerOptions> GetFileServerOptions(ILogger logger, Action<FileServerProviderOptions> configFunc = null)
        {
            List<FileServerOptions> fileServers = new List<FileServerOptions>();

            var foptions = new FileServerProviderOptions();
            if (configFunc != null)
            {
                configFunc(foptions);
            }

            foreach (var dir in foptions.NetworkDirectories)
            {
                try
                {
                    logger.LogInformation("Network Directories: [" + dir.WebPath + "] - [" + dir.NetworkPath + "]");

                    fileServers.Add(
                        new FileServerOptions
                        {
                            FileProvider = new PhysicalFileProvider(dir.NetworkPath),
                            RequestPath = new PathString(dir.WebPath),
                            EnableDirectoryBrowsing = false
                        }
                    );
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to connect to directory: [" + dir.WebPath + "] - [" + dir.NetworkPath + "] " + e.ToString());
                }
            }

            return fileServers;
        }

        private static List<FileServerOptions> GetEmbeddedFileOptions(ILogger logger)
        {
            var fileServers = new List<FileServerOptions>();

            var assList = Assemblies;
            foreach (var ass in assList)
            {
                try
                {
                    var efp = new EmbeddedFileProvider(ass);
                    var dList = efp.GetDirectoryContents("");
                    if (dList.Any())
                    {
                        var nm = Core.GetEmbeddedBasePath(ass);
                        logger.LogInformation("Assembly Directories: [" + ass.GetName().Name + "] [" + nm + "]");
                        foreach (var d in dList)
                        {
                            logger.LogInformation("Assembly Directories: [" +  nm + "] [" + d.PhysicalPath + "] [" + d.Name + "]");
                        }

                        fileServers.Add(
                                    new FileServerOptions
                                    {
                                        FileProvider = efp,
                                        RequestPath = new PathString(Core.GetEmbeddedBasePath(ass)),
                                        EnableDirectoryBrowsing = true
                                    }
                                );
                    }
                }
                catch (Exception e)
                {
                    var a = 1;
                    logger.LogError(e, "Failed to connect to directory: [" + ass.GetName().Name + "] " + e.ToString());
                    //logger.LogError()"Failed to connect to directory: " + dir.Value, e);
                }


            }

            return fileServers;
        }





        public static void InitFileExtensions(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var fileServerprovider = serviceProvider.GetRequiredService<IPhysicalFileServerProvider>();
            //IEmbeddedFileServerProvider


            foreach (var option in fileServerprovider.FileServerOptionsCollection)
            {
                app.UseFileServer(option);
            }

            var embeddedServerprovider = serviceProvider.GetRequiredService<IEmbeddedFileServerProvider>();

            foreach (var option in embeddedServerprovider.FileServerOptionsCollection)
            {
                app.UseFileServer(option);
            }

        }



        private static List<Type> _assemblyTypes = null;
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

        private static List<Assembly> Assemblies
        {
            get
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


                return assemblies;
            }
        }
        private static List<Type> AssemblyTypes
        {
            get
            {
                if (_assemblyTypes == null)
                {

                    var assemblies = Assemblies;
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

                    //_logger.LogDebug("Total Types [" + _assemblyTypes.Count() + "]");
                }
                return _assemblyTypes;
            }
        }

    }
}
