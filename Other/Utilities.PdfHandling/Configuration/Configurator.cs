using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.FileExtensions.Services;

namespace Utilities.PdfHandling.Configuration
{
    public static class Configurator
    {

        public static void InitilizePdfHandling(Action<PdfConfig> setupAction)
        {
            Core.config = Core.config ?? new PdfConfig();
            if (setupAction != null)
            {
                setupAction(Core.config);
            }

        }


        //public static IServiceCollection InitilizePdfHandling(this IServiceCollection services, Action<PdfConfig> setupAction)
        //{
        //    Core.config = Core.config ?? new PdfConfig();
        //    //services.AddTransient<ILocalFileHandling, LocalFileHandler>();
        //    //services.AddTransient<IFullFileHandling, LocalFileHandler>();
        //    //services.AddTransient<IFileHandling, LocalFileHandler>();
        //    ////services.AddSingleton<IFileHandling, TT>();

        //    //var config = new PdfConfig();

        //    if (setupAction!=null)
        //    {
        //        setupAction(Core.config);
        //    }




        //    return services;
        //}

    }
}
