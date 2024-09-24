using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.KeyValueStore
{
    public interface IKeyValueRepository
    {
        void SetSettingValue(string name, string value);
        string GetSettingValue(string name, string defaultValue = "");





    }
}
