using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.ServiceLocator;
using Utilities.ServiceLocator.Interfaces;

namespace Utilities.AutoInitilization.Configuration
{
    public static class Configurator
    {
        public static IServiceCollection RegisterAutoInitilizationServices(this IServiceCollection services)
        {
            services.AddTransient<ILocator, Locator>();

            var sp = services.BuildServiceProvider();
            var locator = sp.GetService<ILocator>();
            var logger = sp.GetService<ILogger>();

            locator.ExecutePerService<IInitilizationService>((s) => {
                logger.LogDebug($"Auto Initilizing: [{s.Name}]");
                s.InitilizeServices(services, sp); 
            }, (s) => -s.Priority);

            return services;
        }
        public static void StartAutoInitilizationServices(this IApplicationBuilder app)
        {
            var sp = app.ApplicationServices;
            var locator = sp.GetService<ILocator>();
            var logger = sp.GetService<ILogger>();

            locator.ExecutePerService<IInitilizationService>((s) => {
                logger.LogDebug($"Auto Initilizing During App Start: [{s.Name}]");
                s.InitilizeServices(app);
            }, (s) => -s.Priority);

        }
    }
}
