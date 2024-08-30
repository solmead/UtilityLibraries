using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.ServiceLocator.Interfaces;

namespace Utilities.AutoInitilization
{
    public interface IInitilizationService : IService
    {
        int Priority { get; }

        IServiceCollection InitilizeServices(IServiceCollection services, IServiceProvider provider);


        void InitilizeServices(IApplicationBuilder app);
    }
}
