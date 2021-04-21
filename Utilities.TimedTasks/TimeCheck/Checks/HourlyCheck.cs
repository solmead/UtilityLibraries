using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.TimedTasks.TimeCheck.Checks
{
    internal class HourlyCheck : ICheck
    {
        public CallRateEnum CallRate => CallRateEnum.Hourly;


        public bool IsTime(ITask task, DateTime lastCheckDate)
        {
            var now = DateTime.Now;
            if (now.Hour != lastCheckDate.Hour || (now.Subtract(lastCheckDate).TotalHours > 2))
            {
                return (now.Minute > task.DayToRun);
            }
            return false;
        }
    }
}
