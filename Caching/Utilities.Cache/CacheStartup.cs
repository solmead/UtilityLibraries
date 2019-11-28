using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Caching;
using Utilities.Caching.Caches;
using Utilities.Caching.Core;

namespace Utilities.Caching.AspNetCore
{
    public static class CacheStartup
    {
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static void InitCache(this IApplicationBuilder app)
        {
            var contextAssesor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            Init(contextAssesor);
        }


        public static void Init(IHttpContextAccessor contextAccessor)
        {
            HttpContext CurrentContext = contextAccessor.HttpContext;
            var system = CacheSystem.Instance;
            system.SetCookieRepository(new CookieRepository(contextAccessor));
            system.CacheAreas[CacheArea.Request] = new RequestCache(contextAccessor);


        }
    }
}
