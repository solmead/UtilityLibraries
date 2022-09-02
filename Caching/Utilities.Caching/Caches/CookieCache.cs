using System.Threading.Tasks;
using Utilities.Caching.Core;

namespace Utilities.Caching.Caches
{
    public class CookieCache : DictionaryCache
    {
        public CookieCache()
            : base(BaseCacheArea.Permanent, ()=> { return Cache.CacheCookie?.CookieId(); }, 60*60*24*60)
        {
            Area =  CacheArea.Cookie;
            Name = "DefaultCookie";
        }



        public override void ClearCache()
        {
            base.ClearCache();
            Cache.CacheCookie?.ResetCookieId();

        }
        public override async Task ClearCacheAsync()
        {
            await base.ClearCacheAsync();
            Cache.CacheCookie?.ResetCookieId();
        }
    }
}
