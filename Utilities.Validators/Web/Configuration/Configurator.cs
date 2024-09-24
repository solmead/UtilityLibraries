using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.AutoInitilization;
using Utilities.KeyValueStore;

namespace Utilities.Validators.Web.Configuration
{

    public class AutoConfigurator : IInitilizationService
    {

        public int Priority => 10000;

        public string Name => "Utilities.Validators.Web.Configuration Service";


        private readonly ILogger<AutoConfigurator> _logger;
        private readonly ISettingsRepository _config;

        //private readonly IMessageBus _messageBus;

        public AutoConfigurator(ILogger<AutoConfigurator> logger, ISettingsRepository config)
        {
            _logger = logger;
            _config = config;
        }

        public IServiceCollection InitilizeServices(IServiceCollection services, IServiceProvider provider)
        {
            var env = provider.GetService<IHostEnvironment>();
            Configurator.InitilizeServices(services, _logger, env, _config);
            return services;
        }

        public void InitilizeServices(IApplicationBuilder app)
        {
            
        }
    }


    public static class Configurator
    {

        //services.AddSingleton<IValidationAttributeAdapterProvider, CustomValidationAttributeAdapterProvider>();
        public static IServiceCollection InitilizeServices(IServiceCollection services, ILogger logger, IHostEnvironment env, ISettingsRepository config)
        {

            services.AddSingleton<IValidationAttributeAdapterProvider, CustomValidationAttributeAdapterProvider>();


            return services;
        }
    }
}
