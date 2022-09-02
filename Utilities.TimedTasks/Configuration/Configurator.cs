using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.TimedTasks.Repos;

namespace Utilities.TimedTasks.Configuration
{
    public static class Configurator
    {
        private static TaskServices __localInstance = null;

        private static object TimedTaskCreateLock = new object();

        private static IServiceProvider? _serviceProvider { get; set; } = null;

        private static ILogger? _logger { get; set; } = null;
        private static ITimedTaskRepository? _timedTaskRepository { get; set; } = null;


        public static TaskServices Instance
        {
            get
            {
                if (_serviceProvider == null && _logger == null && _timedTaskRepository==null)
                {
                    throw new Exception("Must Initialize Timed Tasks before use. Call Utilities.TimedTasks.Configuration.Configurator.InitTimeTasks");
                }
                TaskServices _instance = null;
                if (_serviceProvider != null)
                {
                    _logger = _serviceProvider.GetRequiredService<ILogger>();
                    _timedTaskRepository = _serviceProvider.GetRequiredService<ITimedTaskRepository>();
                    _instance = _serviceProvider.GetRequiredService<TaskServices>();
                }
                else
                {
                    if (__localInstance != null)
                    {
                        _instance = __localInstance;
                    }
                    else
                    {
                        lock (TimedTaskCreateLock)
                        {
                            if (__localInstance != null)
                            {
                                _instance = __localInstance;
                            }
                            else
                            {

                                __localInstance = new TaskServices(_timedTaskRepository, _logger);
                                _instance = __localInstance;
                            }
                        }
                    }
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
            services.AddSingleton<TaskServices>();

        }


        /// <summary>
        /// Initialize Timed Task System
        /// </summary>
        /// <param name="timedTaskRepository"></param>
        /// <param name="logger"></param>
        /// <param name="createTaskFromType">Pass in "DependencyResolver.Current.GetService" if you want to have DI support</param>
        public static void InitTimeTasks(ITimedTaskRepository timedTaskRepository, ILogger logger, Func<Type, ITask>? createTaskFromType = null)
        {
            _timedTaskRepository = timedTaskRepository;
            _logger = logger;
            var it = Instance;


            foreach (var t in Core.GetTasks(createTaskFromType))
            {
                Core.AddTask(t);
            }

        }


        //public static void InitTimeTasks(this IApplicationBuilder app)
        //{
        //    serviceProvider = app.ApplicationServices;



        //    Func<Type, ITask> createTaskFromType = (Type type) =>
        //    {
        //        return app.ApplicationServices.GetService(type) as ITask;
        //    };

        //    foreach (var t in Core.GetTasks(createTaskFromType))
        //    {
        //        Core.AddTask(t);
        //    }
        //}
    }
}
