using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.KeyValueStore;
using Utilities.TimedTasks.Repos;

namespace Utilities.TimedTasks.Configuration
{
    public static class Configurator
    {
        private static TaskServices __localInstance = null;

        private static object TimedTaskCreateLock = new object();

        private static IServiceProvider? _serviceProvider { get; set; } = null;

        private static ILogger? _loggerOld { get; set; } = null;
        private static IKeyValueRepository? _timedTaskRepositoryOld { get; set; } = null;


        public static TaskServices Instance
        {
            get
            {
                if (_serviceProvider == null && _loggerOld == null && _timedTaskRepositoryOld == null)
                {
                    throw new Exception("Must Initialize Timed Tasks before use. Call Utilities.TimedTasks.Configuration.Configurator.InitTimeTasks");
                }
                TaskServices _instance = null;
                if (_serviceProvider != null)
                {
                    if (__localInstance == null)
                    {
                        lock (TimedTaskCreateLock)
                        {
                            if (__localInstance == null)
                            {
                                //_logger = _serviceProvider.GetRequiredService<ILogger>();
                                //_timedTaskRepository = _serviceProvider.GetRequiredService<IKeyValueRepository>();
                                __localInstance = _serviceProvider.GetRequiredService<TaskServices>();
                            }
                        }
                    }
                    _instance = __localInstance;
                    //return _serviceProvider.GetRequiredService<TaskServices>();
                }
                else
                {
                    if (__localInstance == null)
                    {
                        lock (TimedTaskCreateLock)
                        {
                            if (__localInstance != null)
                            {
                                _instance = __localInstance;
                            }
                            else
                            {

                                __localInstance = new TaskServices(_timedTaskRepositoryOld, _loggerOld);
                                _instance = __localInstance;
                            }
                        }
                    }
                    _instance = __localInstance;
                }

                if (_instance == null)
                {
                    throw new Exception("Must Configure Timed Tasks before use. Call Utilities.TimedTasks.Configuration.Configurator.ConfigureTimeTasks");
                }
                return _instance;
            }
        }

        public static void ConfigureTimeTasks(this IServiceCollection services)
        {
            //services.AddSingleton<TaskServices>();
            services.AddScoped<TaskServices>();

        }

        ///// <summary>
        ///// Initialize Timed Task System
        ///// </summary>
        ///// <param name="timedTaskRepository"></param>
        ///// <param name="logger"></param>
        ///// <param name="createTaskFromType">Pass in "DependencyResolver.Current.GetService" if you want to have DI support</param>
        //public static void InitTimeTasks(Func<Type, ITask>? createTaskFromType = null)
        //{
        //    var it = Instance;
        //    foreach (var t in Core.GetTasks(createTaskFromType))
        //    {
        //        Core.AddTask(t);
        //    }

        //}


        /// <summary>
        /// Initialize Timed Task System
        /// </summary>
        /// <param name="timedTaskRepository"></param>
        /// <param name="logger"></param>
        /// <param name="createTaskFromType">Pass in "DependencyResolver.Current.GetService" if you want to have DI support</param>
        public static void InitTimeTasks(this IServiceProvider serviceProvider)
        {
            //_serviceProvider = serviceProvider;
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                _serviceProvider = scope.ServiceProvider; //.GetRequiredService<ILookupService>();

                var it = Instance;

                foreach (var t in Core.GetTasks((t) =>
                {
                    var tsk = ActivatorUtilities.CreateInstance(_serviceProvider, t);
                    return (ITask)tsk;
                }))
                {
                    Core.AddTask(t);
                }
            }
        }


        /// <summary>
        /// Initialize Timed Task System
        /// </summary>
        /// <param name="timedTaskRepository"></param>
        /// <param name="logger"></param>
        /// <param name="createTaskFromType">Pass in "DependencyResolver.Current.GetService" if you want to have DI support</param>
        public static void InitTimeTasks(IKeyValueRepository keyValueRepository, ILogger logger, Func<Type, ITask>? createTaskFromType = null)
        {
            _timedTaskRepositoryOld = keyValueRepository;
            _loggerOld = logger;
            var it = Instance;


            foreach (var t in Core.GetTasks(createTaskFromType))
            {
                Core.AddTask(t);
            }

        }

    }
}
