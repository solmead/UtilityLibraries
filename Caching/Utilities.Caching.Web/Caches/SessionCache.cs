using System.Threading.Tasks;
using Utilities.Caching.Core;

namespace Utilities.Caching.Caches
{
    public class SessionCache : DictionaryCache
    {
        public SessionCache() : base(BaseCacheArea.Global, CacheSystem.SessionId)
        {
            Area =CacheArea.Session;
            Name = "DefaultSession";
            LifeSpanInSeconds = 60*20;
        }


        public override void ClearCache()
        {
            base.ClearCache();
            CacheSystem.Instance.LogDebug("ClearCache: Calling ResetSessionId");
            CacheSystem.ResetSessionId();

        }
        public override async Task ClearCacheAsync()
        {
            await base.ClearCacheAsync();
            CacheSystem.Instance.LogDebug("ClearCacheAsync: Calling ResetSessionId");
            CacheSystem.ResetSessionId();
        }

    }
}
