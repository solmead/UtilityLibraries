﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Caching.Core;
using Utilities.Caching.Helpers;

namespace Utilities.Caching.AspNetCore
{
    public class RequestDataSource : IDataSource
    {
        private static ConcurrentDictionary<string, string> Names = new ConcurrentDictionary<string, string>();
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HttpContext Current => _httpContextAccessor.HttpContext;
        public BaseCacheArea Area { get { return BaseCacheArea.Request; } }

        //public RequestDataSource()
        //{

        //}
        public RequestDataSource(IHttpContextAccessor contextAccessor)
        {
            _httpContextAccessor = contextAccessor;
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

            Names.TryAdd(name.ToUpper(), "");
            var context = Current;
            if (context != null)
            {
                    if (context.Items.ContainsKey(name.ToUpper()))
                    {
                        var t = context.Items[name.ToUpper()] as CachedEntry<tt>;
                    //var t = (CachedEntry<tt>) context.Items[name.ToUpper()];
                        return t;
                    }
            }
            else
            {
                //return Cache.GetItem<CachedEntry<tt>>(CacheArea.Local, name, default(CachedEntry<tt>));
            }
            return default(CachedEntry<tt>);
        }

        public void SetItem<tt>(CachedEntry<tt> item)
        {
            var context = Current;
            if (context != null)
            {
                Names.TryAdd(item.Name.ToUpper(), "");
                //lock (requestSetLock)
                {
                    if (context.Items.ContainsKey(item.Name.ToUpper()))
                    {
                        context.Items.Remove(item.Name.ToUpper());
                    }
                    context.Items.Add(item.Name.ToUpper(), item);
                }
            }
            else
            {
                //Cache.SetItem(CacheArea.Local, item.Name, item);
            }
        }

        public CachedEntry<object> GetItem(string name, Type type)
        {
            var context = Current;
            if (context != null)
            {
                    if (context.Items.ContainsKey(name.ToUpper()))
                    {
                        var t = (CachedEntry<object>)context.Items[name.ToUpper()];
                        return t;
                    }
            }
            else
            {
                //return Cache.GetItem<CachedEntry<object>>(CacheArea.Local, name, default(CachedEntry<object>));
            }
            return default(CachedEntry<object>);
        }

        public void SetItem(Type type, CachedEntry<object> item)
        {
            var context = Current;
            if (context != null)
            {
                //lock (requestSetLock)
                {
                    if (context.Items.ContainsKey(item.Name.ToUpper()))
                    {
                        context.Items.Remove(item.Name.ToUpper());
                    }
                    context.Items.Add(item.Name.ToUpper(), item);
                }
            }
            else
            {
                //Cache.SetItem(CacheArea.Local, item.Name, item);
            }
        }

        public void DeleteItem(string name)
        {
            var context = Current;
            if (context != null)
            {
                Names.TryRemove(name.ToUpper(), out string i);
                //lock (requestSetLock)
                {
                    if (context.Items.ContainsKey(name.ToUpper()))
                    {
                        context.Items.Remove(name.ToUpper());
                    }
                }
            }
        }

        public void DeleteAll()
        {
            var keys = Names.Keys.ToList();
            Names = new ConcurrentDictionary<string, string>();
            foreach (var name in keys)
            {
                DeleteItem(name);
            }
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
