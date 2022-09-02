using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.SerializeExtensions;
using Utilities.TimedTasks.Repos;
using Utilities.TimedTasks.TimeCheck;

namespace Utilities.TimedTasks
{
    internal class TasksHandler
    {

        private readonly ILogger _logger;
        private readonly ITimedTaskRepository _timedTaskRepository;
        private ICheck _checkMethod { get; set; }

        public TasksHandler(ICheck checkMethod, ITimedTaskRepository timedTaskRepository, ILogger logger)
        {
            _checkMethod = checkMethod;
            _logger = logger;
            _timedTaskRepository = timedTaskRepository;
            CallRate = _checkMethod.CallRate;
        }
        public TasksHandler(CallRateEnum callRate, ITimedTaskRepository timedTaskRepository, ILogger logger)
        {
            CallRate = callRate;
            _logger = logger;
            _timedTaskRepository = timedTaskRepository;
            _checkMethod = CheckMethodFactory.GetCheckMethod(callRate);
        }

        public CallRateEnum CallRate { get; set; }



        public DateTime LastCheckDate(string name)
        {
            var s = _timedTaskRepository.GetSettingValue("Global_TimedTasks_" + name + "_Last" + _checkMethod.ToString() + "CheckDate", DateTime.Now.AddYears(-2).ToString());
            DateTime dt;
            DateTime.TryParse(s, out dt);
            _timedTaskRepository.SetSettingValue("Global_TimedTasks_" + name + "_Last" + _checkMethod.ToString() + "CheckDate", dt.ToString());
            return dt;
        }

        public void setLastCheckDate(string name, DateTime value)
        {
            _timedTaskRepository.SetSettingValue("Global_TimedTasks_" + name + "_Last" + _checkMethod.ToString() + "CheckDate", value.ToString());
        }



        public async Task<bool> CheckConcurrency(ITask task)
        {
            var data = new ConcurrencyCheck()
            {
                InstanceId = Core.Instance.InstanceId,
                When = DateTime.Now
            };

            var ser = new Serializer(_logger);


            _timedTaskRepository.SetSettingValue("Global_TimedTasks_" + task.Name + "_ConcurrencyCheck", ser.Serialize(data));

            //Since at this point each instance has written a lastcheckdate, which isTime uses to determine if it is time to run, only those that hit within a very short amount of time could run at the same time. This is to trap the instances where istime returned true before the lastcheckdate got changed.
            await Task.Delay(2000);

            var dt = _timedTaskRepository.GetSettingValue("Global_TimedTasks_" + task.Name + "_ConcurrencyCheck", "");

            var data2 = ser.Deserialize<ConcurrencyCheck>(dt);

            if (data2.InstanceId == Core.Instance.InstanceId)
            {
                _logger.LogInformation("Instance check success on Instance [" + Core.Instance.InstanceId + "] -> Instance Retrieved [" + data2.InstanceId + "]");
                return true;
            }

            //if (DateTime.Now.Subtract(data2.When).TotalMinutes>5)
            //{

            //}

            _logger.LogInformation("Instance check failed on Instance [" + Core.Instance.InstanceId + "] -> Instance Retrieved [" + data2.InstanceId + "]");
            return false;
        }

        public async Task TriggerAsync()
        {
            var list = (from t in Core.Instance.TaskList
                        where t.Rate == CallRate
                        select t).ToList();
            var now = DateTime.Now;
            foreach (var tsk in list)
            {
                var lcd = LastCheckDate(tsk.Name);
                if (_checkMethod.IsTime(tsk, lcd))
                {
                    setLastCheckDate(tsk.Name, DateTime.Now);
                    _logger.LogInformation("Running Task [" + tsk.Name + "]");

                    if (await CheckConcurrency(tsk))
                    {
                        _logger.LogInformation("Running Task [" + tsk.Name + "] on Instance [" + Core.Instance.InstanceId + "]");
                        await tsk.HandleEventAsync(lcd);
                    }
                }
            }

        }





    }

    internal class ConcurrencyCheck {
        public string InstanceId { get; set; }
        public DateTime When { get; set; }
    }
}
