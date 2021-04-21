using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.TimedTasks.TimeCheck.Checks
{
    internal class MinutelyCheck : ICheck
    {
        public CallRateEnum CallRate => CallRateEnum.Minutely;

        public bool IsTime(ITask task, DateTime lastCheckDate)
        {
            var now = DateTime.Now;
            return (now.Minute != lastCheckDate.Minute);
        }
    }
}
