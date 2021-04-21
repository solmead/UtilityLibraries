using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.TimedTasks.TimeCheck.Checks
{
    internal class MonthlyCheck : ICheck
    {
        public CallRateEnum CallRate => CallRateEnum.Monthly;


        public bool IsTime(ITask task, DateTime lastCheckDate)
        {
            var now = DateTime.Now;
            if ((now.Month != lastCheckDate.Month || (now.Subtract(lastCheckDate).TotalDays > 40)) && now.Day == task.DayToRun)
            {
                var dte = new DateTime(now.Year, now.Month, now.Day, task.TimeToRun.Hour, task.TimeToRun.Minute, task.TimeToRun.Second);
                return (dte <= now);
            }
            return false;
        }
    }
}
