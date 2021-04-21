using Utilities.Caching.Core;
using Utilities.Caching.Redis.Models;

namespace Utilities.Caching.Redis
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
