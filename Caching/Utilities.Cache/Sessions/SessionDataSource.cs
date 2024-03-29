﻿using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Caching.Core;
using Utilities.Caching.Helpers;
using Utilities.SerializeExtensions;

namespace Utilities.Caching.AspNetCore.Sessions
{
    public class SessionDataSource : IDataSource
    {

        public static ConcurrentDictionary<string, string> Names = new ConcurrentDictionary<string, string>();
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HttpContext Current => _httpContextAccessor.HttpContext;


        public BaseCacheArea Area { get { return BaseCacheArea.Global; } }


        public SessionDataSource(IHttpContextAccessor contextAccessor)
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
            try
            {
                var ser = new Serializer();
                var ts = Current.Session.GetString(name.ToUpper());
                var t = ser.Deserialize<CachedEntry<tt>>(ts);
                 //   as CachedEntry<tt>;
                return t;
            }
            catch
            {
                //throw;
            }
            return default;
        }

        public void SetItem<tt>(CachedEntry<tt> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            Names.TryAdd(item.Name.ToUpper(), "");
            object comp = item.Item;
            object empty = default(tt);
            if (comp != empty)
            {

                if (item.TimeOut.HasValue && item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds > 0)
                {
                    var lifeSpanSeconds = item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds;

                    int totSeconds = (int)lifeSpanSeconds;
                    int ms = (int)((lifeSpanSeconds - 1.0 * totSeconds) * 1000.0);
                    try
                    {


                        var ser = new Serializer();
                        var ts = ser.Serialize<CachedEntry<tt>>(item);
                        Current.Session.SetString(item.Name.ToUpper(), ts);
                        //HttpContext.Current.Session.Remove(item.Name.ToUpper());


                        //var t = HttpContext.Current.Session[name.ToUpper()] as CachedEntry<tt>;
                        //return t;



                        //lock (_memoryCache)
                        //{
                        //    try
                        //    {
                        //        _memoryCache.Remove(item.Name.ToUpper());
                        //    }
                        //    catch (Exception)
                        //    {

                        //    }
                        //    _memoryCache.Set(item.Name.ToUpper(), item, new TimeSpan(0, 0, 0, totSeconds, ms));
                        //}
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                else
                {
                    var ser = new Serializer();
                    var ts = ser.Serialize<CachedEntry<tt>>(item);
                    Current.Session.SetString(item.Name.ToUpper(), ts);
                    //lock (_memoryCache)
                    //{
                    //    try
                    //    {
                    //        _memoryCache.Remove(item.Name.ToUpper());
                    //    }
                    //    catch (Exception)
                    //    {

                    //    }
                    //    _memoryCache.Set(item.Name.ToUpper(), item);
                    //}
                }


            }
            else
            {
                Current.Session.Remove(item.Name.ToUpper());
                //lock (_memoryCache)
                //{
                //    try
                //    {
                //        _memoryCache.Remove(item.Name.ToUpper());
                //    }
                //    catch (Exception)
                //    {

                //    }
                //}
                ////_memoryCache.Remove(item.Name.ToUpper());
                ////HttpRuntime.Cache.Remove(item.Name.ToUpper());
            }
        }

        public CachedEntry<object> GetItem(string name, Type type)
        {
            Names.TryAdd(name.ToUpper(), "");
            try
            {
                var ser = new Serializer();
                var ts = Current.Session.GetString(name.ToUpper());
                var t = ser.Deserialize<CachedEntry<object>>(ts);
                return t;

                //lock (_memoryCache)
                //{
                //    var t = _memoryCache.Get<CachedEntry<object>>(name.ToUpper());

                //    //var t = (CachedEntry<object>)HttpRuntime.Cache[name.ToUpper()];
                //    return t;
                //}
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
            Names.TryAdd(item.Name.ToUpper(), "");
            object comp = item.Item;
            object empty = null;
            if (comp != empty)
            {
                if (item.TimeOut.HasValue)
                {
                    var lifeSpanSeconds = item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds;
                    int totSeconds = (int)lifeSpanSeconds;
                    int ms = (int)((lifeSpanSeconds - 1.0 * totSeconds) * 1000.0);

                    var ser = new Serializer();
                    var ts = ser.Serialize<CachedEntry<object>>(item);
                    Current.Session.SetString(item.Name.ToUpper(), ts);
                    //lock (_memoryCache)
                    //{
                    //    try
                    //    {
                    //        _memoryCache.Remove(item.Name.ToUpper());
                    //    }
                    //    catch (Exception)
                    //    {

                    //    }
                    //    _memoryCache.Set(item.Name.ToUpper(), item, new TimeSpan(0, 0, 0, totSeconds, ms));
                    //}
                    ////HttpRuntime.Cache.Insert(item.Name.ToUpper(), item, null,
                    ////    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    ////    new TimeSpan(0, 0, 0, totSeconds, ms),
                    ////    CacheItemPriority.Default, null);
                }
                else
                {
                    var ser = new Serializer();
                    var ts = ser.Serialize<CachedEntry<object>>(item);
                    Current.Session.SetString(item.Name.ToUpper(), ts);
                    //lock (_memoryCache)
                    //{
                    //    try
                    //    {
                    //        _memoryCache.Remove(item.Name.ToUpper());
                    //    }
                    //    catch (Exception)
                    //    {

                    //    }
                    //    _memoryCache.Set(item.Name.ToUpper(), item);
                    //    //HttpRuntime.Cache[item.Name.ToUpper()] = item;
                    //}
                }


            }
            else
            {
                Current.Session.Remove(item.Name.ToUpper());
                //lock (_memoryCache)
                //{
                //    try
                //    {
                //        _memoryCache.Remove(item.Name.ToUpper());
                //    }
                //    catch (Exception)
                //    {

                //    }
                //}
                //// _memoryCache.Remove(item.Name.ToUpper());
                ////HttpRuntime.Cache.Remove(item.Name.ToUpper());
            }
        }

        public void DeleteItem(string name)
        {
            Names.TryRemove(name.ToUpper(), out string i);
            try
            {
                Current.Session.Remove(name.ToUpper());
                //lock (_memoryCache)
                //{
                //    _memoryCache.Remove(name.ToUpper());
                //    //HttpRuntime.Cache.Remove(name.ToUpper());
                //}
            }
            catch (Exception)
            {

            }
        }

        public void DeleteAll()
        {
            var keys = Names.Keys.ToList();
            Names = new ConcurrentDictionary<string, string>();
            foreach (var name in keys)
            {
                DeleteItem(name.ToUpper());
            }
        }




        private CachedEntry<tt> LoadItem<tt>(string name, double? lifeSpanSeconds = null)
        {
            var entry = GetItem<tt>(name);
            if (entry == null || entry.TimeOut.HasValue && entry.TimeOut.Value < DateTime.Now)
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

        public async Task<List<tt>> GetListAsync<tt>(string name)
        {
            return GetList<tt>(name);
        }

        public async Task AddToListAsync<tt>(string name, tt item)
        {
            AddToList(name, item);
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
            InsertIntoList(name, index, item);
        }

        public async Task SetInListAsync<tt>(string name, int index, tt item)
        {
            SetInList(name, index, item);
        }

        //public void CopyToList<tt>(string name, tt[] array, int arrayIndex)
        //{
        //    GetList<tt>(name).CopyTo(array, arrayIndex);
        //}
    }
}

