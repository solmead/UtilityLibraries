using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureRedisCaching.Models;
using Utilities.Caching;
using Utilities.Caching.Core;

namespace HttpObjectCaching.Caches
{
    public class AzureRedisCache : DataCache
    {

        public AzureRedisCache()
            : base(new AzureRedisDataSource())
        {
            
        }

        public override CacheArea Area => CacheArea.Distributed;
        public override string Name => "AzureRedisCache";
    }
}
