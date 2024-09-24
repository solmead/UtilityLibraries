using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.KeyValueStore.Concrete;
using Utilities.KeyValueStore.Settings;

namespace Utilities.KeyValueStore.Configuration
{
    public static  class Configurator
    {
        public enum ConfigSettingsEnum
        {
            Standard,
            CheckConfig,
            OnlyConfig
        }

        public static IServiceCollection ConfigureSettings(this IServiceCollection services, ConfigSettingsEnum configSettings = ConfigSettingsEnum.Standard)
        {

            if (configSettings == ConfigSettingsEnum.OnlyConfig)
            {
                services.AddScoped<ISettingsRepository, ConfigurationRepository>();
            }

            if (configSettings== ConfigSettingsEnum.CheckConfig)
            {
                services.AddScoped<ISettingsRepository, SettingsFromConfigRepository>();
            }

            if (configSettings == ConfigSettingsEnum.Standard)
            {
                services.AddScoped<ISettingsRepository, SettingsRepository>();
            }

            return services;
        }
    }
}
