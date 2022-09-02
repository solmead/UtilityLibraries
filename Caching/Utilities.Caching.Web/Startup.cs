using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Caching;
using Utilities.Caching.Core;
using Utilities.Caching.Web.Sessions;
using Utilities.Logging;

[assembly: WebActivatorEx.PreApplicationStartMethod(
    typeof(Utilities.Caching.Web.Startup), "PostStart")]
namespace Utilities.Caching.Web
{
    public static class Startup
    {
        public static void PostStart()
        {
            Init();
        }
        //public static void CacheInit(this IApplicationBuilder app)
        //{
        //    var contextAssesor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
        //    Init(contextAssesor);
        //}


        public static void Init()
        {

            Utilities.Caching.Configuration.Configurator.InitCache(new GenericLogger());
            Utilities.Caching.Configuration.Configurator.SetCookieRepository(new CookieRepository());
            var system = Cache.Instance;
            system.CacheAreas[CacheArea.Session] = new SessionCache();
            system.CacheAreas[CacheArea.Request] = new RequestCache();
        }
    }
}
