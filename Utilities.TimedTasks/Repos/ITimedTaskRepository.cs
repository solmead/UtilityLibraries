using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities.KeyValueStore;

namespace Utilities.TimedTasks.Repos
{
    [Obsolete("Use the base IKeyValueRepository instead", true)]
    public interface ITimedTaskRepository : IKeyValueRepository
    {

    }
}
