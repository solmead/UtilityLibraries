using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Caching.Redis.Configuration
{
    public static class Configurator
    {

        public static string HostName
        {
            get => Cache.GetItem<string>(CacheArea.Global, "CachingRedisHostName", () => "");
            set => Cache.SetItem<string>(CacheArea.Global, "CachingRedisHostName", value);
        }
        public static string CacheKey
        {
            get => Cache.GetItem<string>(CacheArea.Global, "CachingRedisCacheKey", () => "");
            set => Cache.SetItem<string>(CacheArea.Global, "CachingRedisCacheKey", value);
        }
        public static bool AllowNonSSL
        {
            get => Cache.GetItem<bool?>(CacheArea.Global, "CachingRedisAllowNonSSL", () => false) ?? false;
            set => Cache.SetItem<bool?>(CacheArea.Global, "CachingRedisAllowNonSSL", value);
        }
        public static int DefaultTimeoutMinutes
        {
            get => Cache.GetItem<int?>(CacheArea.Global, "CachingRedisDefaultTimeoutMinutes", () => 5) ?? 5;
            set => Cache.SetItem<int?>(CacheArea.Global, "CachingRedisDefaultTimeoutMinutes", value);
        }

        public static void ConfigureRedisCache(this IServiceCollection services)
        {
            //Utilities.Caching.Configuration.Configurator.ConfigureCache(services);
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }


        public static void InitRedisCache(string hostName, string cacheKey, bool allowNonSSL = false, int defaultTimeoutMinutes = 5)//this IApplicationBuilder app)
        {
            HostName = hostName;
            AllowNonSSL = allowNonSSL;
            CacheKey = cacheKey;
            DefaultTimeoutMinutes = Math.Max(defaultTimeoutMinutes, 1);

            var system = Cache.Instance;
        system.CacheAreas[CacheArea.Distributed] = new AzureRedisCache();


        }
    }
}
