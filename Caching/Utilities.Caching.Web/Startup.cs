using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Caching;
using Utilities.Caching.Caches;
using Utilities.Caching.Core;




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
            var system = CacheSystem.Instance;
            system.SetCookieRepository(new CookieRepository());
            system.CacheAreas[CacheArea.Request] = new RequestCache();
        }
    }
}
