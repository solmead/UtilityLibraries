using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities.KeyValueStore;
using Utilities.EnumExtensions;

namespace Utilities.TimedTasks
{
    public abstract class TimedTaskBase : ITask
    {
        protected readonly ILogger _logger;
        protected readonly IKeyValueRepository _keyValueRepository;
        protected DateTime _timeToRun;
        protected DayOfWeek _dayToRun;
        protected CallRateEnum _rate;
        protected bool testing = false;


        public CallRateEnum Rate => _rate;
        public DateTime TimeToRun => _timeToRun;

        public int DayToRun => (int)_dayToRun;

        public abstract string Name { get; } 

        public abstract Func<DateTime, Task> HandleEventAsync { get; }

        protected void PutInTestingMode()
        {

            var now = DateTime.Now;
            _timeToRun = now.AddMinutes(1);
            _dayToRun = now.DayOfWeek;

            _keyValueRepository.SetSettingValue("GLOBAL_TIMEDTASKS_" + Name + "_LASTUTILITIES.TIMEDTASKS.TIMECHECK.CHECKS.WEEKLYCHECKCHECKDATE".ToUpper(), DateTime.Now.AddMonths(-10).ToString());
            _keyValueRepository.SetSettingValue("GLOBAL_TIMEDTASKS_" + Name + "_LASTUTILITIES.TIMEDTASKS.TIMECHECK.CHECKS.DailyCHECKCHECKDATE".ToUpper(), DateTime.Now.AddMonths(-10).ToString());
            _keyValueRepository.SetSettingValue("GLOBAL_TIMEDTASKS_" + Name + "_LASTUTILITIES.TIMEDTASKS.TIMECHECK.CHECKS.MonthlyCHECKCHECKDATE".ToUpper(), DateTime.Now.AddMonths(-10).ToString());


        }
        public TimedTaskBase(IKeyValueRepository keyValueRepository,  ILogger logger,
            CallRateEnum? callRate = CallRateEnum.Weekly,
            DayOfWeek? dayToRun = DayOfWeek.Sunday, 
            string? timeToRun = "2:00 am",
            bool testMode = false)
        {
            try
            {
                _logger = logger;
                _keyValueRepository = keyValueRepository;

                //_timeToRun = new DateTime(1, 1, 1, 2, 0, 0);
                var _t = _keyValueRepository.GetSettingValue(Name + "_Time_To_Run", timeToRun);
                _timeToRun = DateTime.Parse(_t);

                //_dayToRun = DayOfWeek.Sunday;
                var _d = _keyValueRepository.GetSettingValue(Name + "_Day_To_Run", dayToRun.ToString());
                _dayToRun = _d.ToEnum<DayOfWeek>();

                //_rate = CallRateEnum.Daily;
                var _r = _keyValueRepository.GetSettingValue(Name + "_Call_Rate", callRate.ToString());
                _rate = _r.ToEnum<CallRateEnum>();

                testing = Boolean.Parse(_keyValueRepository.GetSettingValue(Name + "_Testing", testMode.ToString()));
                if (testing)
                {
                    PutInTestingMode();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
                var i = 0;
            }

        }
    }
}
