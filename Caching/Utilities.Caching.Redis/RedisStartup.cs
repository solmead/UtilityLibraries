
using HttpObjectCaching.Caches;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Caching;
using Utilities.Caching.Caches;
using Utilities.Caching.Core;

namespace Utilities.Caching.Redis
{
    public static class RedisStartup
    {
        
        public static void Init(string hostName, string cacheKey, bool allowNonSSL = false, int defaultTimeoutMinutes = 5)
        {
            Core.Setup(hostName, cacheKey, allowNonSSL, defaultTimeoutMinutes);


        }
    }
}
