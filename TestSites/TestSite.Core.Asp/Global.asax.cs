using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Utilities.Caching;
using Utilities.Caching.Database;
using Utilities.Caching.Redis;
using Utilities.SerializeExtensions.Serializers;

namespace TestSite.Core.Asp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CacheSystem.Serializer = new XmlSerializer();

            var cString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            DBCacheStartup.Init(cString);
            RedisStartup.Init("nightrideqa.redis.cache.windows.net", "y9UFwgJ+Unjb7grk9EDUz+RMsyMFVjxGCnwJ8D561s0=", false, 480);
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {

            var td = new TestData()
            {
                Id = 1,
                Name = "CAP " + DateTime.Now.ToShortTimeString(),
                Test = new SubTestData()
                {
                    Data = Guid.NewGuid().ToString()
                }
            };



            var c2 = Cache.GetItem<TestData>(CacheArea.Distributed, "Testing", null);
            Cache.SetItem<TestData>(CacheArea.Distributed, "Testing", td);
            c2 = Cache.GetItem<TestData>(CacheArea.Distributed, "Testing", null);

            c2.Id = 2;



            //var c = Cache.GetItem<String>(CacheArea.Cookie, "Name", "CAP2", "");
            //Cache.SetItem<String>(CacheArea.Cookie, "Name", "RCP2");
            //c = Cache.GetItem<String>(CacheArea.Cookie, "Name", "SGP2","");
            //c = c + "";

            //c = Cache.GetItem<String>(CacheArea.Session, "Name", "CAP", "");
            //Cache.SetItem<String>(CacheArea.Session, "Name", "RCP");
            //c = Cache.GetItem<String>(CacheArea.Session, "Name", "SGP", "");
            //c = c + "";


        }
    }
    [Serializable]
    public class TestData
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public SubTestData Test { get; set; }

    }
    [Serializable]
    public class SubTestData
    {
        public string Data { get; set; }
    }
}
