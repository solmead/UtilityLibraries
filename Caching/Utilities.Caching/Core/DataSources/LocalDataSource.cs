using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Caching.Helpers;

namespace Utilities.Caching.Core.DataSources
{
    public class LocalDataSource : IDataSource
    {
        public BaseCacheArea Area { get { return BaseCacheArea.Other; } }
        private ConcurrentDictionary<string, CachedEntryBase> _baseDictionary = new ConcurrentDictionary<string, CachedEntryBase>();
        public int? DefaultTimeOut { get; set; }
        public LocalDataSource()
        {
            
        }
        public LocalDataSource(int? defaultTimeOut)
        {
            DefaultTimeOut= defaultTimeOut;
        }


        public async Task<CachedEntry<tt>> GetItemAsync<tt>(string name)
        {
            return GetItem<tt>(name);
        }

        public async Task SetItemAsync<tt>(CachedEntry<tt> item)
        {
            SetItem(item);
        }

        public async Task<CachedEntry<object>> GetItemAsync(string name, Type type)
        {
            return GetItem(name, type);
        }

        public async Task SetItemAsync(Type type, CachedEntry<object> item)
        {
            SetItem(type, item);
        }

        public async Task DeleteItemAsync(string name)
        {
            DeleteItem(name);
        }

        public async Task DeleteAllAsync()
        {
            DeleteAll();
        }



        public CachedEntry<tt> GetItem<tt>(string name)
        {
            CachedEntryBase itm2;
            CachedEntry<tt> itm;
            _baseDictionary.TryGetValue(name.ToUpper(), out itm2);
            itm = itm2 as CachedEntry<tt>;
            if (itm != null && !itm.TimeOut.HasValue && DefaultTimeOut.HasValue)
            {
                itm.TimeOut = DateTime.Now.AddSeconds(DefaultTimeOut.Value);
            }
            return itm;
        }

        public void SetItem<tt>(CachedEntry<tt> item)
        {
            if (!item.TimeOut.HasValue && DefaultTimeOut.HasValue)
            {
                item.TimeOut = DateTime.Now.AddSeconds(DefaultTimeOut.Value);
            }
            CachedEntryBase itm2;
            _baseDictionary.TryRemove(item.Name.ToUpper(), out itm2);
            _baseDictionary.TryAdd(item.Name.ToUpper(), item);
        }

        public CachedEntry<object> GetItem(string name, Type type)
        {
            CachedEntryBase itm2;
            CachedEntry<object> itm;
            _baseDictionary.TryGetValue(name.ToUpper(), out itm2);
            itm = itm2 as CachedEntry<object>;
            if (itm != null && !itm.TimeOut.HasValue && DefaultTimeOut.HasValue)
            {
                itm.TimeOut = DateTime.Now.AddSeconds(DefaultTimeOut.Value);
            }
            return itm;
        }

        public void SetItem(Type type, CachedEntry<object> item)
        {
            if (!item.TimeOut.HasValue && DefaultTimeOut.HasValue)
            {
                item.TimeOut = DateTime.Now.AddSeconds(DefaultTimeOut.Value);
            }
            CachedEntryBase itm2;
            _baseDictionary.TryRemove(item.Name.ToUpper(), out itm2);
            _baseDictionary.TryAdd(item.Name.ToUpper(), item);
        }

        public void DeleteItem(string name)
        {
            CachedEntryBase itm2;
            _baseDictionary.TryRemove(name.ToUpper(), out itm2);
        }

        public void DeleteAll()
        {
            //throw new NotImplementedException();
        }



        private CachedEntry<tt> LoadItem<tt>(string name, double? lifeSpanSeconds = null)
        {
            var entry = GetItem<tt>(name);
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
        public List<tt> GetList<tt>(string name)
        {
            var lstEntry = LoadItem<List<tt>>(name);
            if (lstEntry.Item == null)
            {
                lstEntry.Item = new List<tt>();
                SetItem(lstEntry);
            }
            return lstEntry.Item;
        }

        public void AddToList<tt>(string name, tt item)
        {
            GetList<tt>(name).Add(item);
        }

        public void ClearList<tt>(string name)
        {
            GetList<tt>(name).Clear();
        }

        public void RemoveFromList<tt>(string name, tt item)
        {
            GetList<tt>(name).Remove(item);
        }

        public void RemoveFromListAt<tt>(string name, int index)
        {
            GetList<tt>(name).RemoveAt(index);
        }

        public void InsertIntoList<tt>(string name, int index, tt item)
        {
            GetList<tt>(name).Insert(index, item);
        }

        public void SetInList<tt>(string name, int index, tt item)
        {
            GetList<tt>(name)[index] = item;
        }

        public void CopyToList<tt>(string name, tt[] array, int arrayIndex)
        {
            GetList<tt>(name).CopyTo(array, arrayIndex);
        }
        public async Task<List<tt>> GetListAsync<tt>(string name)
        {
            return GetList<tt>(name);
        }

        public async Task AddToListAsync<tt>(string name, tt item)
        {
            AddToList<tt>(name, item);
        }

        public async Task ClearListAsync<tt>(string name)
        {
            ClearList<tt>(name);
        }

        public async Task RemoveFromListAsync<tt>(string name, tt item)
        {
            RemoveFromList(name, item);
        }

        public async Task RemoveFromListAtAsync<tt>(string name, int index)
        {
            RemoveFromListAt<tt>(name, index);
        }

        public async Task InsertIntoListAsync<tt>(string name, int index, tt item)
        {
            InsertIntoList<tt>(name, index, item);
        }

        public async Task SetInListAsync<tt>(string name, int index, tt item)
        {
            SetInList(name, index, item);
        }

    }
}
