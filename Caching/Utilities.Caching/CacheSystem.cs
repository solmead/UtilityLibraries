
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Caching.CacheAreas;
using Utilities.Caching.Caches;
using Utilities.Caching.Core;
using Utilities.SerializeExtensions;
using Utilities.SerializeExtensions.Serializers;

namespace Utilities.Caching
{
    public class CacheSystem
    {
        private readonly ILogger _logger;

        public Dictionary<CacheArea, ICacheArea> CacheAreas { get; private set; }

        public List<TaggedCacheEntry> TaggedEntries { get; set; } = new List<TaggedCacheEntry>();

        public bool CacheEnabled { get; set; }

        


        //public Action<string> LogErrorMessage { get; set; }
        //public Action<string> LogDebugMessage { get; set; }

        public void LogDebug(string msg)
        {
            _logger.LogDebug(msg);   
        }
        public void LogError(string msg)
        {
            _logger.LogError(msg);
            //LogErrorMessage?.Invoke(msg);
        }
        //public static void SetLogDebugFunction(Action<string> logCall)
        //{
        //    CacheSystem.Instance.LogDebugMessage = logCall;
        //}
        //public static void SetLogErrorFunction(Action<string> logCall)
        //{
        //    CacheSystem.Instance.LogErrorMessage = logCall;
        //}


        //public static CacheSystem Instance
        //{

        //    get
        //    {

        //        //IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        //        var ctx = _instance;
        //        if (ctx == null)
        //        {
        //            lock (CacheSystemCreateLock)
        //            {
        //                ctx = _instance;
        //                if (ctx == null)
        //                {
        //                    ctx = new CacheSystem();
        //                    _instance = ctx;
        //                    //_memoryCache.Set("CurrentCacheInstance", ctx);
        //                    //ctx = _memoryCache.Get<CacheSystem>("CurrentCacheInstance");
        //                }
        //            }
        //        }
        //        return ctx;
        //    }
        //}





        public void AddTaggedEntry(CacheArea cacheArea, string tags, string entryName)
        {
            if (string.IsNullOrWhiteSpace(tags) || string.IsNullOrWhiteSpace(entryName))
            {
                return;
            }
            var te = (from t in TaggedEntries.ToList()
                      where t.CacheArea == cacheArea && t.EntryName.ToUpper() == entryName.ToUpper()
                      select t).FirstOrDefault();

            if (te == null)
            {
                te = new TaggedCacheEntry()
                {
                    CacheArea = cacheArea,
                    EntryName = entryName,
                    Tags = ""
                };
                TaggedEntries.Add(te);
            }

            if (string.IsNullOrWhiteSpace(tags))
            {
                tags = "";
            }
            if (string.IsNullOrWhiteSpace(te.Tags))
            {
                te.Tags = "";
            }




            var tarr = (tags.ToUpper() + "," + te.Tags.ToUpper()).Split(',').ToList();
            tarr = (from t in tarr where !string.IsNullOrWhiteSpace(t) select t.Trim()).Distinct().ToList();
            tarr.Insert(0, "");
            tarr.Add("");
            te.Tags = String.Join(",", tarr);

        }


        //public CacheSystem(ILogger logger, ICookieRepository cookieRepository) : this()
        //{
        //    CookieRepository = cookieRepository;

        //    _logger = logger;
        //}

        public CacheSystem(ILogger logger) : this()
        {

            _logger = logger;
        }
        private CacheSystem()
        {
            CacheEnabled = true;

            //
            CacheAreas = new Dictionary<CacheArea, ICacheArea>();
            //CacheAreas.Add(CacheArea.Request, new RequestCache(_httpContextAccessor));
            CacheAreas.Add(CacheArea.Request, new NoCache());
            //CacheAreas.Add(CacheArea.Session, new SessionCache());
            CacheAreas.Add(CacheArea.Session, new NoCache());
            CacheAreas.Add(CacheArea.Permanent, new NoCache());
            CacheAreas.Add(CacheArea.Global, new GlobalCache());
            CacheAreas.Add(CacheArea.Cookie, new CookieCache());
            CacheAreas.Add(CacheArea.Distributed, new NoCache());
            CacheAreas.Add(CacheArea.None, new NoCache());
            CacheAreas.Add(CacheArea.Other, new NoCache());
            CacheAreas.Add(CacheArea.Temp, new NoCache());

        }


        public ICacheArea GetCacheArea(CacheArea area)
        {
            if (CacheAreas.ContainsKey(area))
            {
                return CacheAreas[area];
            }
            //area doesn't exist, go through each level till we find a level that works.
            var maxval = (from int v in Enum.GetValues(typeof(CacheArea)) select v).Max();
            for (var a = (int)area; a <= maxval; a++)
            {
                var ea = (CacheArea)a;
                if (CacheAreas.ContainsKey(ea))
                {
                    return CacheAreas[ea];
                }
            }

            return null;
        }
        public async Task ClearAllCacheAreasAsync()
        {
          
            foreach (var area in CacheAreas.Keys)
            {
                try
                {
                    await GetCacheArea(area).ClearCacheAsync();
                }
                catch (NotImplementedException)
                {

                }
            }
        }
        public List<TaggedCacheEntry> GetTaggedCacheEntries(string tag)
        {
            return (from t in TaggedEntries
                where t.Tags.ToUpper().Contains("," + tag.ToUpper() + ",")
                select t).ToList();
        }

        public async Task ClearTaggedCacheAsync(string tag)
        {
            var tList = GetTaggedCacheEntries(tag);
            foreach (var t in tList)
            {
                try { 
                    await Cache.ClearItemAsync(t.CacheArea, t.EntryName);
                }
                catch
                {

                }
            }
        }

        public async Task ClearCacheAsync(CacheArea area)
        {
            try
            {
                await GetCacheArea(area).ClearCacheAsync();
            } catch
            {

            }
        }

        public async Task<IDictionary<string, object>> GetDataDictionaryAsync(CacheArea area)
        {
            var nvl = GetCacheArea(area) as INameValueLister;
            if (nvl != null)
            {
                return await nvl.DataDictionaryGetAsync();
            }
            return null;
        }
        public void ClearAllCacheAreas()
        {
            foreach (var area in CacheAreas.Keys)
            {
                try
                {
                    GetCacheArea(area).ClearCache();
                }
                catch (NotImplementedException)
                {

                }
            }
        }

        public void ClearTaggedCache(string tag)
        {
            try
            {
                var tList = GetTaggedCacheEntries(tag);
                foreach (var t in tList)
                {
                    try
                    {
                        Cache.ClearItem(t.CacheArea, t.EntryName);
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }
        }

        public void ClearCache(CacheArea area)
        {
            try { 
                GetCacheArea(area).ClearCache();
            }
            catch
            {

            }
        }

        public IDictionary<string, object> GetDataDictionary(CacheArea area)
        {
            var nvl = GetCacheArea(area) as INameValueLister;
            if (nvl != null)
            {
                return nvl.DataDictionaryGet();
            }
            return null;
        }






    }
}
