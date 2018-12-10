using Utilities.Caching.Core;
using Utilities.Caching.Core.DataSources;

namespace Utilities.Caching.Caches
{
    //public class GlobalCache : DictionaryCache
    //{
    //    public GlobalCache()
    //        : base(new DataCache(new ApplicationDataSource()), () => "global")
    //    {
    //        Area = CacheArea.Global;
    //        Name = "DefaultGlobal";
    //    }
    //}
    public class GlobalCache : DataCache
    {
        public GlobalCache()
            : base(new ApplicationDataSource())
        {
            Area = CacheArea.Global;
            Name = "DefaultGlobal";
        }
    }
}
