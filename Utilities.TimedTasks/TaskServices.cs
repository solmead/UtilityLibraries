using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.EnumExtensions;
using Utilities.KeyValueStore;
using Utilities.TimedTasks.Quartz;
using Utilities.TimedTasks.Repos;

namespace Utilities.TimedTasks
{
    public class TaskServices
    {
        

        internal string InstanceId { get; set; } = Guid.NewGuid().ToString();

        private List<TasksHandler> Handlers { get; set; } = new List<TasksHandler>();

        private bool IsTriggering = false;

        private readonly ILogger _logger;

        private QuartzSchedulerThread schedulerThread;

        internal List<ITask> TaskList = new List<ITask>();
        private bool disposedValue;


        private static object TimedTaskCreateLock = new object();

        public TaskServices(IKeyValueRepository timedTaskRepository,ILogger logger)
        {
            _logger = logger;

            //Utilities.EnumExtensions.Extensions.GetWithOrder<CallRateEnum>();

            CallRateEnum cre = CallRateEnum.Minutely;

            cre.GetWithOrder().ToList().ForEach((cr) =>
            {
                Handlers.Add(new TasksHandler(cr, timedTaskRepository, _logger));
            });
            //Handlers


            schedulerThread = new QuartzSchedulerThread(_logger);

            schedulerThread.Start();

        }

        public ITask? FindTask(string name)
        {
            return TaskList.FirstOrDefault((r) => r.Name.ToUpper().Trim() == name.ToUpper().Trim());
        }

        public void AddTask(ITask task)
        {
            _logger.LogInformation("Adding Task [" + task.Name + "] to tasks list");

            if (TaskList.Any((r) => r.Name == task.Name))
            {
                var t = TaskList.First(r => r.Name == task.Name);
                TaskList.Remove(t);
            }
            TaskList.Add(task);
        }

        public async Task TriggerAsync()
        {
            if (IsTriggering)
            {
                return;
            }

            await Task.Delay(1005);

            lock (TimedTaskCreateLock)
            {
                if (IsTriggering)
                {
                    return;
                }
                IsTriggering = true;
            }


            try
            {
                foreach (var hand in Handlers)
                {
                    await hand.TriggerAsync();
            {
                try
                {
                    await TriggerAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            });
            t.Start();
        }


    }
}

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            IsTriggering = false;


        }

        public void TriggerTasks()
        {
            var t = new Task(async () =>