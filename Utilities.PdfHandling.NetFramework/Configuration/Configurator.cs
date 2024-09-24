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

        public static void InitilizePdfHandling(this IKernel kernel, Action<PdfConfig> setupAction)
        {

            kernel.Bind<IPdfCreation>().To<PdfCreation>();


            Core.config = Core.config ?? new PdfConfig();
            if (setupAction != null)
            {
                setupAction(Core.config);
            }

        }


    }
}
