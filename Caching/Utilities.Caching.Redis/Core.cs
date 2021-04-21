using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;

namespace Utilities.Caching.Redis
{
    public class Core
    {


        //public static string HostName
        //{
        //    get => Cache.GetItem<string>(CacheArea.Global, "CachingRedisHostName", () => "");
        //    set => Cache.SetItem<string>(CacheArea.Global, "CachingRedisHostName", value);
        //}
        //public static string CacheKey
        //{
        //    get => Cache.GetItem<string>(CacheArea.Global, "CachingRedisCacheKey", () => "");
        //    set => Cache.SetItem<string>(CacheArea.Global, "CachingRedisCacheKey", value);
        //}
        //public static bool AllowNonSSL
        //{
        //    get => Cache.GetItem<bool?>(CacheArea.Global, "CachingRedisAllowNonSSL", () => false) ?? false;
        //    set => Cache.SetItem<bool?>(CacheArea.Global, "CachingRedisAllowNonSSL", value);
        //}
        //public static int DefaultTimeoutMinutes
        //{
        //    get => Cache.GetItem<int?>(CacheArea.Global, "CachingRedisDefaultTimeoutMinutes", () => 5) ?? 5;
        //    set => Cache.SetItem<int?>(CacheArea.Global, "CachingRedisDefaultTimeoutMinutes", value);
        //}

        //public static void Setup()
        //{
        //    var applicationSettings = ConfigurationManager.GetSection("ApplicationSettings") as NameValueCollection;

        //    ConfigurationManager.


        //    if (applicationSettings.Count == 0)
        //    {
        //        Console.WriteLine("Application Settings are not defined");
        //    }
        //    else
        //    {
        //        var settingsArea = applicationSettings["AzureRedisCaching.Properties.Settings"];
        //        //HostName = settingsArea

        //        foreach (var key in applicationSettings.AllKeys)
        //        {
        //            Console.WriteLine(key + " = " + applicationSettings[key]);
        //        }
        //    }


        //    HostName = hostName;
        //    AllowNonSSL = allowNonSSL;
        //    CacheKey = cacheKey;
        //    DefaultTimeoutMinutes = defaultTimeoutMinutes;

        //    var system = CacheSystem.Instance;
        //    system.CacheAreas[CacheArea.Distributed] = new AzureRedisCache();
        //}
        [Obsolete("Use Utilities.Caching.Redis.Configuration.InitRedisCache", true)]
        public static void Setup(string hostName, string cacheKey, bool allowNonSSL = false, int defaultTimeoutMinutes = 5)
        {
            


        }

    }
}
