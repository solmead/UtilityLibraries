using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.TimedTasks.TimeCheck
{
    internal interface ICheck
    {

        CallRateEnum CallRate { get; }
        bool IsTime(ITask task, DateTime lastCheckDate);
    }
}
