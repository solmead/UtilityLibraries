using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.FileExtensions.AzureBlob.Configuration
{
    public static class Configurator
    {
        public static IServiceCollection ConfigureBlobFileHandling(this IServiceCollection services, BlobOptions options)
        {
            options = options ?? new BlobOptions();
            services.AddSingleton(options);

            services.AddSingleton<IFileHandling, BlobFileHandler>();
            services.AddSingleton<IFullFileHandling, BlobFileHandler>();


            return services;
        }
    }
}
