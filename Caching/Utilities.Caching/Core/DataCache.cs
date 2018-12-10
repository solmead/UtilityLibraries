using System;
using System.Threading.Tasks;
using Utilities.Caching.CacheAreas;
using Utilities.Caching.Helpers;

namespace Utilities.Caching.Core
{
    public class DataCache : ICacheArea
    {
        private IDataSource _dataSource = null;

        public DataCache(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public IDataSource DataSource { get { return _dataSource; } }

        public virtual CacheArea Area { get; protected set; }
        public virtual string Name { get; protected set; }
        public void ClearCache()
        {
             _dataSource.DeleteAll();
        }
        public async Task ClearCacheAsync()
        {
            await _dataSource.DeleteAllAsync();
        }
        
        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null, string tags = "")
        {

            CacheSystem.Instance.AddTaggedEntry(Area, tags, name);


            object empty = default(tt);
            tt tObj = default(tt);
            var entry =  LoadItem<tt>(name, lifeSpanSeconds);
            try
            {
                tObj = (tt)(entry.Item);
            }
            catch (Exception)
            {

            }
            object comp = tObj;
            if (comp == empty)
            {
                if (createMethod != null)
                {
                    tObj =  createMethod();
                    entry.Item = tObj;
                    SaveItem(entry);
                }
            }
            return tObj;
        }


        private async Task<Object> LoadItemAsync(string name, Type type, Func<Task<object>> createMethod = null, double? lifeSpanSeconds = null)
        {
            object empty = null;// default(tt);
            object tObj = null;
            var entry = await LoadItemAsync(name, type, lifeSpanSeconds);
            try
            {
                tObj = entry.Item;
            }
            catch (Exception)
            {

            }
            object comp = tObj;
            if (comp == empty)
            {
                if (createMethod != null)
                {
                    tObj = await createMethod();
                    entry.Item = tObj;
                    await SaveItemAsync(entry);
                }
            }
            return tObj;
        }

        //public object GetItemAsync(string name, Type type, Func<Task<object>> createMethod = null, double? lifeSpanSeconds = null)
        //{
        //    Type d1 = typeof(Task<>);
        //    Type[] typeArgs = { type };
        //    Type constructed = d1.MakeGenericType(typeArgs);
        //    //var ob = delegate { return CacheSystem.Instance; }
        //    var t = new Task<CacheSystem>(() => { return CacheSystem.Instance; });

        //    object o = Activator.CreateInstance(constructed, BindingFlags.Default, null, );
        //    var i = LoadItemAsync(name, type, createMethod, lifeSpanSeconds);
        //    return o;
        //}
        public async Task<tt> GetItemAsync<tt>(string name, Func<Task<tt>> createMethod = null, double? lifeSpanSeconds = null, string tags = "")
        {
            CacheSystem.Instance.AddTaggedEntry(Area, tags, name);

            object empty = default(tt);
            tt tObj = default(tt);
            var entry = await LoadItemAsync<tt>(name, lifeSpanSeconds);
            try
            {
                tObj = (tt)(entry.Item);
            }
            catch (Exception)
            {

            }
            object comp = tObj;
            if (comp == empty)
            {
                if (createMethod != null)
                {
                    tObj = await createMethod();
                    entry.Item = tObj;
                    await SaveItemAsync(entry);
                }
            }
            return tObj;
        }

        private CachedEntry<tt> LoadItem<tt>(string name, double? lifeSpanSeconds = null)
        {
            var entry =  _dataSource.GetItem<tt>(name);
            if (entry == null || (entry.TimeOut.HasValue && entry.TimeOut.Value < DateTime.Now))
            {
                entry = new CachedEntry<tt>()
                {
                    Name = name,
                    Changed = DateTime.Now,
                    Created = DateTime.Now
                };
                if (lifeSpanSeconds.HasValue)
                {
                    entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
                }
            }
            return entry;
        }
        private async Task<CachedEntry<object>> LoadItemAsync(string name, Type type, double? lifeSpanSeconds = null)
        {
            var entry = await _dataSource.GetItemAsync<object>(name);
            if (entry == null || (entry.TimeOut.HasValue && entry.TimeOut.Value < DateTime.Now))
            {
                entry = new CachedEntry<object>()
                {
                    Name = name,
                    Changed = DateTime.Now,
                    Created = DateTime.Now
                };
                if (lifeSpanSeconds.HasValue)
                {
                    entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
                }
            }
            return entry;
        }
        private async Task<CachedEntry<tt>> LoadItemAsync<tt>(string name, double? lifeSpanSeconds = null)
        {
            var entry = await _dataSource.GetItemAsync<tt>(name);
            if (entry == null || (entry.TimeOut.HasValue && entry.TimeOut.Value < DateTime.Now))
            {
                entry = new CachedEntry<tt>()
                {
                    Name = name,
                    Changed = DateTime.Now,
                    Created = DateTime.Now
                };
                if (lifeSpanSeconds.HasValue)
                {
                    entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
                }
            }
            return entry;
        }
        //private CachedEntry<object> LoadItem(string name, Type type, double? lifeSpanSeconds = null)
        //{
        //    var entry = _dataSource.GetItem(name, type);
        //    if (entry == null || (entry.TimeOut.HasValue && entry.TimeOut.Value < DateTime.Now))
        //    {
        //        entry = new CachedEntry<object>()
        //        {
        //            Name = name,
        //            Changed = DateTime.Now,
        //            Created = DateTime.Now
        //        };
        //        if (lifeSpanSeconds.HasValue)
        //        {
        //            entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
        //        }
        //    }
        //    return entry;
        //}
        //private async Task<CachedEntry<object>> LoadItemAsync(string name, Type type, double? lifeSpanSeconds = null)
        //{
        //    var entry = await _dataSource.GetItemAsync(name, type);
        //    if (entry == null || (entry.TimeOut.HasValue && entry.TimeOut.Value < DateTime.Now))
        //    {
        //        entry = new CachedEntry<object>()
        //        {
        //            Name = name,
        //            Changed = DateTime.Now,
        //            Created = DateTime.Now
        //        };
        //        if (lifeSpanSeconds.HasValue)
        //        {
        //            entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
        //        }
        //    }
        //    return entry;
        //}
        private void SaveItem<tt>(CachedEntry<tt> entry)
        {
             _dataSource.SetItem(entry);
        }
        private async Task SaveItemAsync<tt>(CachedEntry<tt> entry)
        {
            await _dataSource.SetItemAsync(entry);
        }
        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null, string tags = "")
        {
            CacheSystem.Instance.AddTaggedEntry(Area, tags, name);

            var entry =  LoadItem<tt>(name, lifeSpanSeconds);
            entry.Item = obj;
            entry.Changed = DateTime.Now;
            if (lifeSpanSeconds.HasValue)
            {
                entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
            }
             SaveItem(entry);
        }
        public async Task SetItemAsync<tt>(string name, tt obj, double? lifeSpanSeconds = null, string tags = "")
        {

            CacheSystem.Instance.AddTaggedEntry(Area, tags, name);

            var entry = await LoadItemAsync<tt>(name, lifeSpanSeconds);
            entry.Item = obj;
            entry.Changed = DateTime.Now;
            if (lifeSpanSeconds.HasValue)
            {
                entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
            }
            await SaveItemAsync(entry);
        }

        public object GetItemAsync(string name, Type type, Func<Task<object>> createMethod = null, double? lifeSpanSeconds = null, string tags = "")
        {
            CacheSystem.Instance.AddTaggedEntry(Area, tags, name);

            return null;
        }


        //public object GetItem(string name, Type type, Func<object> createMethod = null, double? lifeSpanSeconds = null)
        //{
        //    //object empty = default(tt);
        //    object tObj = null;
        //    CachedEntry<object> entry =  LoadItem(name, type, lifeSpanSeconds);

        //    try
        //    {
        //        object itm = entry.Item;
        //        if (type.IsInstanceOfType(itm))
        //        {
        //            tObj = itm;
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    object comp = tObj;
        //    if (comp == null)
        //    {
        //        if (createMethod != null)
        //        {
        //            tObj =  createMethod();
        //            entry.Item = tObj;
        //            //await SaveItemAsync(entry);
        //             _dataSource.SetItem(type, entry);
        //        }
        //    }
        //    return tObj;
        //}
        //public async Task<object> GetItemAsync(string name, Type type, Func<Task<object>> createMethod = null, double? lifeSpanSeconds = null)
        //{
        //    //object empty = default(tt);
        //    object tObj = null;
        //    CachedEntry<object> entry = await LoadItemAsync(name, type, lifeSpanSeconds);

        //    try
        //    {
        //        object itm = entry.Item;
        //        if (type.IsInstanceOfType(itm))
        //        {
        //            tObj = itm;
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    object comp = tObj;
        //    if (comp == null)
        //    {
        //        if (createMethod != null)
        //        {
        //            tObj = await createMethod();
        //            entry.Item = tObj;
        //            //await SaveItemAsync(entry);
        //            await _dataSource.SetItemAsync(type, entry);
        //        }
        //    }
        //    return tObj;
        //}

        //public void SetItem(string name, Type type, object obj, double? lifeSpanSeconds = null)
        //{
        //    var entry =  LoadItem(name, type, lifeSpanSeconds);
        //    entry.Item = obj;
        //    entry.Changed = DateTime.Now;
        //    if (lifeSpanSeconds.HasValue)
        //    {
        //        entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
        //    }
        //     _dataSource.SetItem(type, entry);
        //}
        //public async Task SetItemAsync(string name, Type type, object obj, double? lifeSpanSeconds = null)
        //{
        //    var entry = await LoadItemAsync(name, type, lifeSpanSeconds);
        //    entry.Item = obj;
        //    entry.Changed = DateTime.Now;
        //    if (lifeSpanSeconds.HasValue)
        //    {
        //        entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
        //    }
        //    await _dataSource.SetItemAsync(type, entry);
        //}
    }
}
