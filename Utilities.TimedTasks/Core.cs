using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.TimedTasks.Repos;
using Microsoft.Extensions.DependencyInjection;
using Utilities.TimedTasks.Configuration;

namespace Utilities.TimedTasks
{
    public static class Core
    {

        //private static ILogger _logger;
        private static bool IsInTaskCheck { get; set; }

       // private static List<ITask> taskList = new List<ITask>();

        private static object assemLoadLock = new object();

        private static List<Type> assemblyTypes { get; set; }



        internal static List<Type> getAssemblyTypes()
        {
            if (assemblyTypes == null)
            {
                lock (assemLoadLock)
                {
                    if (assemblyTypes == null)
                    {
                        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

                        var tList = new List<Type>();
                        foreach (var ass in assemblies)
                        {
                            try
                            {
                                var types = ass.GetTypes();
                                tList.AddRange(types);
                            }
                            catch //(Exception ex2)
                            {
                                //_logger.LogError(ex2, ex2.Message);
                                //var i = 0;
                            }
                        }

                        assemblyTypes = tList;
                    }
                }
            }
            return assemblyTypes;
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
                    var types = getAssemblyTypes();
                    var lst = (from t in types
                                where t.GetInterfaces().Contains(typeof(ITask))
                                select creator(t)).ToList();
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
        public static TaskServices Instance
        {
            get
            {
                if (Configurator.serviceProvider == null)
                {
                    throw new Exception("Must Initialize Timed Tasks before use. Call Utilities.TimedTasks.Core.InitTimeTasks");
                }

                var _instance = Configurator.serviceProvider.GetRequiredService<TaskServices>();
                if (_instance==null)
                {
                    throw new Exception("Must Configure Timed Tasks before use. Call Utilities.TimedTasks.Core.ConfigureTimeTasks");
                }
                return _instance;
            }
        }

        //public static void Initilize(ITimedTaskRepository timedTaskRepository, ILogger logger)
        //{
        //    logger.LogInformation("Initilizing Timed Tasks Services");
        //    _instance = new TaskServices(timedTaskRepository, logger);
        //}

        


        ///// <summary>
        ///// Initialize Timed Task System
        ///// </summary>
        ///// <param name="timedTaskRepository"></param>
        ///// <param name="logger"></param>
        ///// <param name="createTaskFromType">Pass in "DependencyResolver.Current.GetService" if you want to have DI support</param>
        //public static void Initilize(ITimedTaskRepository timedTaskRepository, ILogger logger, Func<Type, ITask> createTaskFromType = null)
        //{
        //    TaskServices.Initilize(timedTaskRepository, logger);


        //    foreach(var t in GetTasks(createTaskFromType))
        //    {
        //        AddTask(t);
        //    }

        //}







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
