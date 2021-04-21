using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.TimedTasks.TimeCheck.Checks
{
    internal class YearlyCheck : ICheck
    {
        public CallRateEnum CallRate => CallRateEnum.Yearly;


        public bool IsTime(ITask task, DateTime lastCheckDate)
        {
            var now = DateTime.Now;
            if (now.Year != lastCheckDate.Year && now.DayOfYear == task.DayToRun)
            {
                var dte = new DateTime(now.Year, now.Month, now.Day, task.TimeToRun.Hour, task.TimeToRun.Minute, task.TimeToRun.Second);
                return (dte <= now);
            }
            return false;
        }
    }
}
