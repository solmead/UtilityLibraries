using Microsoft.Extensions.DependencyInjection;
using Ninject;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.FileExtensions.Services;
using Utilities.PdfHandling.NetFramework.Concretes;

namespace Utilities.PdfHandling.NetFramework.Configuration
{
    public static class Configurator
    {
        public static PdfConfig config = null;

        public static void InitilizePdfHandling(this IKernel kernel, Action<PdfConfig> setupAction)
        {

            kernel.Bind<IPdfCreation>().To<PdfCreation>();


            config = config ?? new PdfConfig();
            if (setupAction != null)
            {
                setupAction(config);
            }

        }


    }
}
