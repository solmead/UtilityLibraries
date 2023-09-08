using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.FileExtensions.Services;

namespace Utilities.FileExtensions.Configuration
{
    public static class Configurator
    {

        public static IServiceCollection InitilizeFileExtensions(this IServiceCollection services)
            //where TT:class,IFileHandling
        {

            services.AddTransient<LocalFileHandler, LocalFileHandler>();
            services.AddTransient<ILocalFileHandling, LocalFileHandler>();
            services.AddTransient<IFullFileHandling, LocalFileHandler>();
            services.AddTransient<IFileHandling, LocalFileHandler>();
            //services.AddSingleton<IFileHandling, TT>();

            return services;
        }

    }
}
