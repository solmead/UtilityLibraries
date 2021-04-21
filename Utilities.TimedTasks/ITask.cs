using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.TimedTasks
{
    public enum CallRateEnum
    {
        //None,
        Minutely,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
    public interface ITask
    {

        string Name { get; }
        CallRateEnum Rate { get; }

        DateTime TimeToRun { get; }

        int DayToRun { get; }


        Func<DateTime, Task> HandleEventAsync { get; }

    }
}
