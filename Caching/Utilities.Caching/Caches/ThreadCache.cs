//using Utilities.Caching.Core;
//using Utilities.Caching.Core.DataSources;

//namespace Utilities.Caching.Caches
//{
//    public class ThreadCache : DictionaryCache
//    {
//        public ThreadCache()
//            : base(new DataCache(new ThreadDataSource()),async () => "thread", 1)
//        {
//            Area = CacheArea.Thread;
//            Name = "DefaultThread";
//        }
//    }
//}
