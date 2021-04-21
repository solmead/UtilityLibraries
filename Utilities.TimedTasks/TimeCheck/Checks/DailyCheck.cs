using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.TimedTasks.TimeCheck.Checks
{
    internal class DailyCheck : ICheck
    {
        public CallRateEnum CallRate => CallRateEnum.Hourly;


        public bool IsTime(ITask task, DateTime lastCheckDate)
        {
            var now = DateTime.Now;
            if (now.Day != lastCheckDate.Day || (now.Subtract(lastCheckDate).TotalDays > 2))
            {
                var dte = new DateTime(now.Year, now.Month, now.Day, task.TimeToRun.Hour, task.TimeToRun.Minute, task.TimeToRun.Second);
                return (dte <= now);
            }
            return false;
        }
    }
}
