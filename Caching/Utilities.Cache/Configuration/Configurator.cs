using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Caching.AspNetCore.Configuration
{
    public static class Configurator
    {
        public static void ConfigureCache(this IServiceCollection services)
        {
            Utilities.Caching.Configuration.Configurator.ConfigureCache(services);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }


        public static void InitCache(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var contextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var logger = serviceProvider.GetRequiredService<ILogger>();
            HttpContext CurrentContext = contextAccessor.HttpContext;
            Utilities.Caching.Configuration.Configurator.InitCache(serviceProvider);
            Utilities.Caching.Configuration.Configurator.SetCookieRepository(new CookieRepository(contextAccessor));

              var system = Cache.Instance;
            system.CacheAreas[CacheArea.Session] = new Sessions.SessionCache(contextAccessor, logger);
            system.CacheAreas[CacheArea.Request] = new RequestCache(contextAccessor);
        }

    }
}
