using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.EnumExtensions;
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

        public TaskServices(ITimedTaskRepository timedTaskRepository,ILogger logger)
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

            await Task.Delay(1000);

            IsTriggering = true;

            try
            {
                foreach(var hand in Handlers)
                {
                    await hand.TriggerAsync();
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
