using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.KeyValueStore.Concrete;

namespace Utilities.KeyValueStore.Settings
{
    public class ConfigurationRepository : SettingsFromConfigRepository, IKeyValueRepository
    {
        public ConfigurationRepository(IConfiguration configuration, ILogger logging) : base(configuration, logging)
        {
            KeyValueRepository = this;
        }

        public string GetSettingValue(string name, string defaultValue = "")
        {
            return defaultValue;
        }

        public void SetSettingValue(string name, string value)
        {

        }
    }
}
