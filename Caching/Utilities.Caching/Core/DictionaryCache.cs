using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Caching.CacheAreas;
using Utilities.Caching.Helpers;

namespace Utilities.Caching.Core
{
    public abstract class DictionaryCache : ICacheArea, INameValueLister
    {
        //private object createLock = new object();
        //private object setLock = new object();
        private ICacheArea _cache = null;

        //private ICacheArea _baseTempCache = new NoCache();

        public ICacheArea CacheTo
        {
            get
            {
                if (_cache != null)
                {
                    return _cache;
                }
                return CacheSystem.Instance.GetCacheArea((CacheArea) CacheToType);
            }
        }
        public IDataSource DataSource { get { return CacheTo.DataSource; } }
        public BaseCacheArea CacheToType { get; set; }
        private Func<string> GetInstanceId = null;


        public double LifeSpanInSeconds { get; set; }

        public DictionaryCache(BaseCacheArea cacheTo, Func<string> getInstanceId, double lifeSpanInSeconds = 30*60)
        {
            LifeSpanInSeconds = lifeSpanInSeconds;
            CacheToType = cacheTo;
            GetInstanceId = getInstanceId;
        }

        public DictionaryCache(ICacheArea cacheTo, Func<string> getInstanceId, double lifeSpanInSeconds = 30*60)
        {
            LifeSpanInSeconds = lifeSpanInSeconds;
            _cache = cacheTo;
            GetInstanceId = getInstanceId;
        }

        public CacheArea Area { get; protected set; }
        public string Name { get; protected set; }


        //public IDictionary<string, object> DataDictionary
        //{
        //    get
        //    {
        //        return AsyncHelper.RunSync(DataDictionaryGet);
        //    }
        //}

        public IDictionary<string, object> DataDictionaryGet()
        {
            //System.Collections.Concurrent.ConcurrentDictionary<>
            var dic = new Dictionary<string, object>();
            foreach (var ce in ( BaseDictionaryGet()).Values)
            {
                if (!dic.ContainsKey(ce.Name.ToUpper()))
                {
                    dic.Add(ce.Name.ToUpper(), ce.TheObject);
                }
            }
            return dic;
        }
        public async Task<IDictionary<string, object>> DataDictionaryGetAsync()
        {
            //System.Collections.Concurrent.ConcurrentDictionary<>
            var dic = new Dictionary<string, object>();
            foreach (var ce in (await BaseDictionaryGetAsync()).Values)
            {
                if (!dic.ContainsKey(ce.Name.ToUpper()))
                {
                    dic.Add(ce.Name.ToUpper(), ce.TheObject);
                }
            }
            return dic;
        }

        private ConcurrentDictionary<string, CachedEntryBase> BaseDictionaryGet()
        {
            var curname = Name + "_DataDictionary_Base_" + GetInstanceId();
            return CacheTo.GetItem(curname, () => new ConcurrentDictionary<string, CachedEntryBase>(), LifeSpanInSeconds);
        }
        private async Task<ConcurrentDictionary<string, CachedEntryBase>> BaseDictionaryGetAsync()
        {
            var curname = Name + "_DataDictionary_Base_" + GetInstanceId();
            return await CacheTo.GetItemAsync(curname,async () => new ConcurrentDictionary<string, CachedEntryBase>(), LifeSpanInSeconds);
        }

        private async Task BaseDictionarySetAsync(ConcurrentDictionary<string, CachedEntryBase> value)
        {
            var curname = Name + "_DataDictionary_Base_" +  GetInstanceId();
            await CacheTo.SetItemAsync<ConcurrentDictionary<string, CachedEntryBase>>(curname, value, LifeSpanInSeconds);
        }
        private void BaseDictionarySet(ConcurrentDictionary<string, CachedEntryBase> value)
        {
            var curname = Name + "_DataDictionary_Base_" +  GetInstanceId();
            CacheTo.SetItem<ConcurrentDictionary<string, CachedEntryBase>>(curname, value, LifeSpanInSeconds);
        }





        public void ClearCache()
        {
            ( BaseDictionaryGet()).Clear();
            //await BaseDictionarySetAsync(null);
        }
        public async Task ClearCacheAsync()
        {
            (await BaseDictionaryGetAsync()).Clear();
            //await BaseDictionarySetAsync(null);
        }

        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null, string tags = "")
        {
            CacheSystem.Instance.AddTaggedEntry(Area, tags, name);

            //using (StringLock.Lock.AcquireLock(Name + "_DataDictionary_Base_" + name.ToUpper()))
            //{

            var dd = BaseDictionaryGet();
                object empty = default(tt);
                CachedEntryBase itm2;
                CachedEntry<tt> itm;
                dd.TryGetValue(name.ToUpper(), out itm2);
                itm = itm2 as CachedEntry<tt>;
                //if (dd.TryGetValue())
                //var itm = dd.getByName(name.ToUpper()) as CachedEntry<tt>;
                if (itm == null || itm.ItemObject == empty || (itm.TimeOut.HasValue && itm.TimeOut.Value < DateTime.Now))
                {
                    if (createMethod != null)
                    {
                        var t = createMethod();
                        SetItem(name, t, lifeSpanSeconds);
                        return t;
                    }
                }
                else
                {
                    return (tt) itm.Item;
                }
                return default(tt);
            //}
        }
        public async Task<tt> GetItemAsync<tt>(string name, Func<Task<tt>> createMethod = null, double? lifeSpanSeconds = null, string tags = "")
        {
            CacheSystem.Instance.AddTaggedEntry(Area, tags, name);


            var dd = await BaseDictionaryGetAsync();
            object empty = default(tt);
            CachedEntryBase itm2;
            CachedEntry<tt> itm;
            dd.TryGetValue(name.ToUpper(), out itm2);
            itm = itm2 as CachedEntry<tt>;
            //if (dd.TryGetValue())
            //var itm = dd.getByName(name.ToUpper()) as CachedEntry<tt>;
            if (itm == null || itm.ItemObject == empty || (itm.TimeOut.HasValue && itm.TimeOut.Value < DateTime.Now))
            {
                if (createMethod != null)
                {
                    var t = await createMethod();
                    await SetItemAsync(name, t, lifeSpanSeconds);
                    return t;
                }
            }
            else
            {
                return (tt)itm.Item;
            }
            return default(tt);
        }
        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null, string tags = "")
        {
            CacheSystem.Instance.AddTaggedEntry(Area, tags, name);
            //using (StringLock.Lock.AcquireLock(Name + "_DataDictionary_Base_" + name.ToUpper()))
            //{

            var dd = BaseDictionaryGet();
                CachedEntryBase itm2 = null;
                CachedEntry<tt> itm = null;
                dd.TryGetValue(name.ToUpper(), out itm2);
                itm = itm2 as CachedEntry<tt>;
                //var itm = dd.getByName(name.ToUpper()) as CachedEntry<tt>;
                if (itm == null)
                {
                    itm = new CachedEntry<tt>()
                    {
                        Created = DateTime.Now,
                        Name = name,
                        Changed = DateTime.Now
                    };
                    if (!dd.TryAdd(name.ToUpper(), itm))
                    {
                        dd.TryGetValue(name.ToUpper(), out itm2);
                        itm = itm2 as CachedEntry<tt>;
                    }
                }
                if (itm != null)
                {
                    if (lifeSpanSeconds.HasValue)
                    {
                        itm.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
                    }
                    itm.Changed = DateTime.Now;
                    itm.Item = obj;
                }

                var lst =
                    (from ce in dd.Values where ce.TimeOut.HasValue && ce.TimeOut.Value < DateTime.Now select ce).ToList
                        ();
                if (lst.Count > 0)
                {
                    foreach (var dit in lst)
                    {
                        dd.TryRemove(dit.Name.ToUpper(), out itm2);
                    }
                }
                BaseDictionarySet(dd);
            //}
        }
        public async Task SetItemAsync<tt>(string name, tt obj, double? lifeSpanSeconds = null, string tags = "")
        {
            CacheSystem.Instance.AddTaggedEntry(Area, tags, name);

            var dd = await BaseDictionaryGetAsync();
            CachedEntryBase itm2 = null;
            CachedEntry<tt> itm = null;
            dd.TryGetValue(name.ToUpper(), out itm2);
            itm = itm2 as CachedEntry<tt>;
            //var itm = dd.getByName(name.ToUpper()) as CachedEntry<tt>;
            if (itm == null)
            {
                itm = new CachedEntry<tt>()
                {
                    Created = DateTime.Now,
                    Name = name,
                    Changed = DateTime.Now
                };
                if (!dd.TryAdd(name.ToUpper(), itm))
                {
                    dd.TryGetValue(name.ToUpper(), out itm2);
                    itm = itm2 as CachedEntry<tt>;
                }
            }
            if (itm != null)
            {
                if (lifeSpanSeconds.HasValue)
                {
                    itm.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
                }
                itm.Changed = DateTime.Now;
                itm.Item = obj;
            }

            var lst =
                (from ce in dd.Values where ce.TimeOut.HasValue && ce.TimeOut.Value < DateTime.Now select ce).ToList
                    ();
            if (lst.Count > 0)
            {
                foreach (var dit in lst)
                {
                    dd.TryRemove(dit.Name.ToUpper(), out itm2);
                }
            }
            await BaseDictionarySetAsync(dd);
        }

        //public async Task<object> GetItemAsync(string name, Type type, Func<Task<object>> createMethod = null, double? lifeSpanSeconds = null)
        //{
        //    var dd = await BaseDictionaryGetAsync();
        //    object empty = null;
        //    CachedEntryBase itm2;
        //    CachedEntry<object> itm;
        //    dd.TryGetValue(name.ToUpper(), out itm2);
        //    itm = itm2 as CachedEntry<object>;
        //    //if (dd.TryGetValue())
        //    //var itm = dd.getByName(name.ToUpper()) as CachedEntry<tt>;
        //    if (itm == null || itm.ItemObject == empty || (itm.TimeOut.HasValue && itm.TimeOut.Value < DateTime.Now))
        //    {
        //        if (createMethod != null)
        //        {
        //            var t = await createMethod();
        //            await SetItemAsync(name, t, lifeSpanSeconds);
        //            return t;
        //        }
        //    }
        //    else
        //    {
        //        return itm.Item;
        //    }
        //    return null;
        //}

        //public async Task SetItemAsync(string name, Type type, object obj, double? lifeSpanSeconds = null)
        //{
        //    var dd = await BaseDictionaryGetAsync();
        //    CachedEntryBase itm2 = null;
        //    CachedEntry<object> itm = null;
        //    dd.TryGetValue(name.ToUpper(), out itm2);
        //    itm = itm2 as CachedEntry<object>;
        //    //var itm = dd.getByName(name.ToUpper()) as CachedEntry<tt>;
        //    if (itm == null)
        //    {
        //        itm = new CachedEntry<object>()
        //        {
        //            Created = DateTime.Now,
        //            Name = name,
        //            Changed = DateTime.Now
        //        };
        //        if (!dd.TryAdd(name.ToUpper(), itm))
        //        {
        //            dd.TryGetValue(name.ToUpper(), out itm2);
        //            itm = itm2 as CachedEntry<object>;
        //        }
        //    }
        //    if (itm != null)
        //    {
        //        if (lifeSpanSeconds.HasValue)
        //        {
        //            itm.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
        //        }
        //        itm.Changed = DateTime.Now;
        //        itm.Item = obj;
        //    }

        //    var lst =
        //        (from ce in dd.Values where ce.TimeOut.HasValue && ce.TimeOut.Value < DateTime.Now select ce).ToList();
        //    if (lst.Count > 0)
        //    {
        //        foreach (var dit in lst)
        //        {
        //            dd.TryRemove(dit.Name.ToUpper(), out itm2);
        //        }
        //    }
        //    await BaseDictionarySetAsync(dd);
        //}



        //public object GetItem(string name, Type type, Func<object> createMethod = null, double? lifeSpanSeconds = null)
        //{
        //    var dd =  BaseDictionaryGet();
        //    object empty = null;
        //    CachedEntryBase itm2;
        //    CachedEntry<object> itm;
        //    dd.TryGetValue(name.ToUpper(), out itm2);
        //    itm = itm2 as CachedEntry<object>;
        //    //if (dd.TryGetValue())
        //    //var itm = dd.getByName(name.ToUpper()) as CachedEntry<tt>;
        //    if (itm == null || itm.ItemObject == empty || (itm.TimeOut.HasValue && itm.TimeOut.Value < DateTime.Now))
        //    {
        //        if (createMethod != null)
        //        {
        //            var t =  createMethod();
        //             SetItem(name, t, lifeSpanSeconds);
        //            return t;
        //        }
        //    }
        //    else
        //    {
        //        return itm.Item;
        //    }
        //    return null;
        //}

        //public void SetItem(string name, Type type, object obj, double? lifeSpanSeconds = null)
        //{
        //    var dd =  BaseDictionaryGet();
        //    CachedEntryBase itm2 = null;
        //    CachedEntry<object> itm = null;
        //    dd.TryGetValue(name.ToUpper(), out itm2);
        //    itm = itm2 as CachedEntry<object>;
        //    //var itm = dd.getByName(name.ToUpper()) as CachedEntry<tt>;
        //    if (itm == null)
        //    {
        //        itm = new CachedEntry<object>()
        //        {
        //            Created = DateTime.Now,
        //            Name = name,
        //            Changed = DateTime.Now
        //        };
        //        if (!dd.TryAdd(name.ToUpper(), itm))
        //        {
        //            dd.TryGetValue(name.ToUpper(), out itm2);
        //            itm = itm2 as CachedEntry<object>;
        //        }
        //    }
        //    if (itm != null)
        //    {
        //        if (lifeSpanSeconds.HasValue)
        //        {
        //            itm.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
        //        }
        //        itm.Changed = DateTime.Now;
        //        itm.Item = obj;
        //    }

        //    var lst =
        //        (from ce in dd.Values where ce.TimeOut.HasValue && ce.TimeOut.Value < DateTime.Now select ce).ToList();
        //    if (lst.Count > 0)
        //    {
        //        foreach (var dit in lst)
        //        {
        //            dd.TryRemove(dit.Name.ToUpper(), out itm2);
        //        }
        //    }
        //     BaseDictionarySet(dd);
        //}
    }
}

