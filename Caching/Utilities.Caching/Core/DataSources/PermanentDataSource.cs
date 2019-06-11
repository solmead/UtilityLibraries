using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Utilities.Caching.Core.Extras;
using Utilities.Caching.Helpers;

namespace Utilities.Caching.Core.DataSources
{
    public class PermanentDataSource : IDataSource
    {

        public BaseCacheArea Area => BaseCacheArea.Permanent;

        public IPermanentRepository CacheRepo { get; set; }

        //public PermanentDataSource()
        //{
            
        //}
        public PermanentDataSource(IPermanentRepository baseData)
        {
            CacheRepo = baseData;
        }


        public async Task<CachedEntry<tt>> GetItemAsync<tt>(string name)
        {
            try
            {

                var t = await CacheRepo.GetAsync(name.ToUpper());
                return CacheSystem.Serializer.Deserialize<CachedEntry<tt>>(t);
            }
            catch
            {
                //throw;
            }
            return default(CachedEntry<tt>);
        }

        public async Task SetItemAsync<tt>(CachedEntry<tt> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            object comp = item.Item;
            object empty = default(tt);
            if (comp != empty)
            {
                var s = CacheSystem.Serializer.SerializeToArray(item);
                if (item.TimeOut.HasValue)
                {
                    await CacheRepo.SetAsync(item.Name.ToUpper(), s, item.TimeOut.Value.Subtract(DateTime.Now));
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s,
                    //    item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds);
                }
                else
                {
                    await CacheRepo.SetAsync(item.Name.ToUpper(), s, null);
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s);
                }
            }
            else
            {
                await DeleteItemAsync(item.Name);
            }
        }

        public async Task<CachedEntry<object>> GetItemAsync(string name, Type type)
        {
            try
            {
                var t = await CacheRepo.GetAsync(name.ToUpper());
                return CacheSystem.Serializer.Deserialize(t, type) as CachedEntry<object>;
            }
            catch
            {
                //throw;
            }
            return null;
        }

        public async Task SetItemAsync(Type type, CachedEntry<object> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            object comp = item.Item;
            object empty = null;
            if (comp != empty)
            {
                var s = CacheSystem.Serializer.SerializeToArray(item, type);
                if (item.TimeOut.HasValue)
                {
                    await CacheRepo.SetAsync(item.Name.ToUpper(), s, item.TimeOut.Value.Subtract(DateTime.Now));
                }
                else
                {
                    await CacheRepo.SetAsync(item.Name.ToUpper(), s, null);
                }
            }
            else
            {
                await DeleteItemAsync(item.Name);
            }
        }

        public async Task DeleteItemAsync(string name)
        {
            await CacheRepo.DeleteAsync(name.ToUpper());
        }

        public async Task DeleteAllAsync()
        {
            var keys = await CacheRepo.GetKeysAsync();
            foreach (var key in keys)
            {
                Console.WriteLine("Removing Key {0} from cache", key.ToString());
                await CacheRepo.DeleteAsync(key);
            }
        }





        public CachedEntry<tt> GetItem<tt>(string name)
        {
            try
            {
                var t = CacheRepo.Get(name.ToUpper());
                return CacheSystem.Serializer.Deserialize<CachedEntry<tt>>(t);
            }
            catch
            {
                //throw;
            }
            return default(CachedEntry<tt>);
        }

        public void SetItem<tt>(CachedEntry<tt> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            object comp = item.Item;
            object empty = default(tt);
            if (comp != empty)
            {
                var s = CacheSystem.Serializer.SerializeToArray(item);
                if (item.TimeOut.HasValue)
                {
                    CacheRepo.Set(item.Name.ToUpper(), s, item.TimeOut.Value.Subtract(DateTime.Now));
                }
                else
                {
                    CacheRepo.Set(item.Name.ToUpper(), s, null);
                }
            }
            else
            {
                DeleteItem(item.Name);
            }
        }

        public CachedEntry<object> GetItem(string name, Type type)
        {
            try
            {
                var t = CacheRepo.Get(name.ToUpper());
                return CacheSystem.Serializer.Deserialize(t, type) as CachedEntry<object>;
            }
            catch
            {
                //throw;
            }
            return null;
        }

        public void SetItem(Type type, CachedEntry<object> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            object comp = item.Item;
            object empty = null;
            if (comp != empty)
            {
                var s = CacheSystem.Serializer.SerializeToArray(item, type);
                if (item.TimeOut.HasValue)
                {
                    CacheRepo.Set(item.Name.ToUpper(), s, item.TimeOut.Value.Subtract(DateTime.Now));
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s,
                    //    item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds);
                }
                else
                {
                    CacheRepo.Set(item.Name.ToUpper(), s, null);
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s);
                }
            }
            else
            {
                DeleteItem(item.Name);
            }
        }

        public void DeleteItem(string name)
        {
            CacheRepo.Delete(name.ToUpper());
            //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + name, null);
        }

        public void DeleteAll()
        {

            var keys = CacheRepo.GetKeys();
            foreach (var key in keys)
            {
                Console.WriteLine("Removing Key {0} from cache", key.ToString());
                CacheRepo.Delete(key);
            }
        }

        //private string GetStringOfItem<tt>(tt item)
        //{
        //    return JsonConvert.SerializeObject(item);
        //}

        //private tt GetItemOfString<tt>(string val)
        //{
        //    return JsonConvert.DeserializeObject<tt>(val);
        //}

        //private bool IsInList<tt>(tt item)
        //{
        //    string v = GetStringOfItem<tt>(item);
        //    CacheDatabase.L
        //}

        private void WriteLine(string msg)
        {
            Debug.WriteLine(DateTime.Now.ToLongTimeString() + " : " + DateTime.Now.Millisecond + " - " + msg);
        }

        public List<tt> GetList<tt>(string name)
        {
            //WriteLine("AzureRedis GetList:" + name);

            //var lst = CacheDatabase.ListRange(name).ToList();
            //return (from i in lst select GetItemOfString<tt>(i.ToString())).ToList();
            throw new NotImplementedException();
        }

        public void AddToList<tt>(string name, tt item)
        {
            //WriteLine("AzureRedis AddToList:" + name + " [" + item.ToString() + "]");
            //string v = GetStringOfItem<tt>(item);
            //CacheDatabase.ListRightPush(name, v);
            throw new NotImplementedException();
        }

        public void ClearList<tt>(string name)
        {
            //WriteLine("AzureRedis ClearList:" + name);
            //CacheDatabase.ListTrim(name, 0, 0);
            //CacheDatabase.ListLeftPop(name);
            throw new NotImplementedException();
        }

        public void RemoveFromList<tt>(string name, tt item)
        {
            //WriteLine("AzureRedis RemoveFromList:" + name + " [" + item.ToString() + "]");
            //string v = GetStringOfItem<tt>(item);
            //CacheDatabase.ListRemove(name, v);
            throw new NotImplementedException();
        }

        public void RemoveFromListAt<tt>(string name, int index)
        {
            //WriteLine("AzureRedis RemoveFromListAt:" + name + " [" + index + "]");
            //var v = CacheDatabase.ListGetByIndex(name, index);
            //CacheDatabase.ListRemove(name, v);
            throw new NotImplementedException();
        }

        public void InsertIntoList<tt>(string name, int index, tt item)
        {
            //WriteLine("AzureRedis Insert into List:" + name + " - " + index + " [" + item.ToString() + "]");
            //var vPivot = CacheDatabase.ListGetByIndex(name, index);
            //string v = GetStringOfItem<tt>(item);
            //CacheDatabase.ListInsertAfter(name, vPivot, v);
            throw new NotImplementedException();
        }

        public void SetInList<tt>(string name, int index, tt item)
        {
            //WriteLine("AzureRedis SetInList:" + name + " - " + index + " [" + item.ToString() + "]");
            //string v = GetStringOfItem<tt>(item);
            //CacheDatabase.ListSetByIndex(name, index, v);
            throw new NotImplementedException();
        }

        public async Task<List<tt>> GetListAsync<tt>(string name)
        {
            //WriteLine("AzureRedis GetList:" + name);

            //var lst = (await CacheDatabase.ListRangeAsync(name)).ToList();
            //return (from i in lst select GetItemOfString<tt>(i.ToString())).ToList();
            throw new NotImplementedException();
        }

        public async Task AddToListAsync<tt>(string name, tt item)
        {
            //WriteLine("AzureRedis AddToList:" + name + " [" + item.ToString() + "]");
            //string v = GetStringOfItem<tt>(item);
            //await CacheDatabase.ListRightPushAsync(name, v);
            throw new NotImplementedException();
        }

        public async Task ClearListAsync<tt>(string name)
        {
            //WriteLine("AzureRedis ClearList:" + name);
            //await CacheDatabase.ListTrimAsync(name, 0, 0);
            //await CacheDatabase.ListLeftPopAsync(name);
            throw new NotImplementedException();
        }

        public async Task RemoveFromListAsync<tt>(string name, tt item)
        {
            //WriteLine("AzureRedis RemoveFromList:" + name + " [" + item.ToString() + "]");
            //string v = GetStringOfItem<tt>(item);
            //await CacheDatabase.ListRemoveAsync(name, v);
            throw new NotImplementedException();
        }

        public async Task RemoveFromListAtAsync<tt>(string name, int index)
        {
            //WriteLine("AzureRedis RemoveFromListAt:" + name + " [" + index + "]");
            //var v = await CacheDatabase.ListGetByIndexAsync(name, index);
            //await CacheDatabase.ListRemoveAsync(name, v);
            throw new NotImplementedException();
        }

        public async Task InsertIntoListAsync<tt>(string name, int index, tt item)
        {
            //WriteLine("AzureRedis Insert into List:" + name + " - " + index + " [" + item.ToString() + "]");
            //var vPivot = await CacheDatabase.ListGetByIndexAsync(name, index);
            //string v = GetStringOfItem<tt>(item);
            //await CacheDatabase.ListInsertAfterAsync(name, vPivot, v);
            throw new NotImplementedException();
        }

        public async Task SetInListAsync<tt>(string name, int index, tt item)
        {
            //WriteLine("AzureRedis SetInList:" + name + " - " + index + " [" + item.ToString() + "]");
            //string v = GetStringOfItem<tt>(item);
            //await CacheDatabase.ListSetByIndexAsync(name, index, v);
            throw new NotImplementedException();
        }
    }
}
