using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.SerializeExtensions;
using Utilities.Poco;
using Microsoft.Extensions.Logging;

namespace Utilities.KeyValueStore.Concrete
{
    public class SettingsFromConfigRepository : SettingsRepository
    {

        private readonly IConfiguration _configuration;

        public SettingsFromConfigRepository(IConfiguration configuration, ILogger logging) : base(logging)
        {
            _configuration = configuration;
        }

        public SettingsFromConfigRepository(IConfiguration configuration, ILogger logging, IKeyValueRepository keyValueRepository) : base(logging, keyValueRepository) 
        {
            _configuration = configuration;
        }

        public override TT GetValue<TT>(string name, TT defaultValue = null) where TT : class
        {
            var d = _configuration.GetSection(name).Get<TT>();
            d = d ?? defaultValue;
            return base.GetValue<TT>(name, d);
        }


        public override TT GetValueSeperate<TT>(string name, TT defaultValue = null) where TT : class
        {
            var d = _configuration.GetSection(name).Get<TT>();
            d = d ?? defaultValue;
            return base.GetValueSeperate<TT>(name, d);
        }

        public override string GetValueString(string name, string defaultValue = "")
        {
            var d = _configuration[name];
            d = d ?? defaultValue;
            return base.GetValueString(name, d);
        }

    }
}
