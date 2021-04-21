using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
            Utilities.Caching.Configuration.Configurator.InitCache(serviceProvider);

            var contextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            HttpContext CurrentContext = contextAccessor.HttpContext;
              var system = Cache.Instance;
            system.SetCookieRepository(new CookieRepository(contextAccessor));
            system.CacheAreas[CacheArea.Session] = new Sessions.SessionCache(contextAccessor);
            system.CacheAreas[CacheArea.Request] = new RequestCache(contextAccessor);
        }

    }
}
