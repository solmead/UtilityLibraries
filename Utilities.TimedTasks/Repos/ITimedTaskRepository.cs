using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.TimedTasks.Repos
{
    public interface ITimedTaskRepository
    {
        void SetSettingValue(string name, string value);
        string GetSettingValue(string name, string defaultValue = "");
    }
}
