using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using Utilities.FileExtensions.Services;
using Utilities.KeyValueStore;
using Utilities.PdfHandling.NetCore.Concretes;

namespace Utilities.PdfHandling.NetCore.Configuration
{
    public static class Configurator
    {
        public static IServiceCollection InitilizePdfHandling(this IServiceCollection services, IHostEnvironment env, ILogger logger, ISettingsRepository config)
        {
            UC.PdfServices.Client.Configuration.Configurator.InitilizeUserProfileClientServices(services, logger, env,
                config);

            services.AddTransient<IPdfCreation, PdfCreation>();

            return services;
        }

    }
}
