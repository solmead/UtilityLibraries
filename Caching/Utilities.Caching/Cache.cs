using System;
using System.Threading.Tasks;
using Utilities.Caching.CacheAreas;
using Utilities.Caching.Core;

namespace Utilities.Caching
{
    public enum CacheArea
    {
        Other = 0,
        Permanent = 1,
        Distributed = 2,
        Global = 3,
        Cookie = 4,
        Session = 5,
        Temp = 6,
        Request = 7,
        //Local = 8,
        //Thread = 7,
        None = 9
    }
    public enum BaseCacheArea
    {
        Other = 0,
        Permanent = 1,
        Distributed = 2,
        Global = 3,
        Temp = 6,
        Request = 7,
        //Local = 8,
        //Thread = 7,
        None = 9
    }
    public static class Cache
    {

        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(ICacheArea area, string name, Func<Task<tt>> createMethod, double lifeSpanSeconds, string tags = "")
        {
            try
            {

                double? lSS = lifeSpanSeconds;
                if (lSS == 0)
                {
                    lSS = null;
                }
                if (!name.Contains("CacheEnabled") && !CacheSystem.Instance.CacheEnabled)
                {
                    if (createMethod != null)
                    {
                        return await createMethod();
                    }
                    return default(tt);
                }
                if (area != null)
                {
                    //name = area.Name + "_" + name;
                    //if (area.Area < CacheArea.Temp)
                    //{
                    //    return await GetItemAsync(CacheArea.Temp, name + "_Request_TempCache", async () =>
                    //    {
                    //        return await area.GetItemAsync<tt>(name, createMethod, lSS);
                    //    });
                    //}
                    //else
                    {
                        return await area.GetItemAsync<tt>(name, createMethod, lSS, tags);
                    }
                }
                if (createMethod != null)
                {
                    return await createMethod();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error:" + ex.Message + " retrieving cache item [" + name + "] on cache [" + area.ToString() + "]", ex);
            }
            return default(tt);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static tt GetItem<tt>(ICacheArea area, string name, Func<tt> createMethod, double lifeSpanSeconds, string tags = "")
        {
            try
            {

                double? lSS = lifeSpanSeconds;
                if (lSS == 0)
                {
                    lSS = null;
                }
                if (!name.Contains("CacheEnabled") && !CacheSystem.Instance.CacheEnabled)
                {
                    if (createMethod != null)
                    {
                        return  createMethod();
                    }
                    return default(tt);
                }

                if (area != null)
                {
                    //name = area.Name + "_" + name;
                    //if (area.Area < CacheArea.Temp)
                    //{
                    //    return GetItem(CacheArea.Temp, name + "_Request_TempCache", () =>
                    //    {
                    //        return  area.GetItem<tt>(name, createMethod, lSS);
                    //    });
                    //}
                    //else
                    {
                        return  area.GetItem<tt>(name, createMethod, lSS, tags);
                    }
                }
                if (createMethod != null)
                {
                    return  createMethod();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error:" + ex.Message + " retrieving cache item [" + name + "] on cache [" + area.ToString() + "]", ex);
            }
            return default(tt);
        }

        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static async Task SetItemAsync<tt>(ICacheArea area, string name, tt obj, double lifeSpanSeconds, string tags = "")
        {
            double? lSS = lifeSpanSeconds;
            if (lSS == 0)
            {
                lSS = null;
            }
            //name = area.Name + "_" + name;
            await area.SetItemAsync<tt>(name, obj, lSS, tags);
            //if (area.Area < CacheArea.Temp)
            //{
            //    await SetItemAsync(CacheArea.Temp, name + "_Request_TempCache", obj);
            //}


        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static void SetItem<tt>(ICacheArea area, string name, tt obj, double lifeSpanSeconds, string tags = "")
        {
            double? lSS = lifeSpanSeconds;
            if (lSS == 0)
            {
                lSS = null;
            }
            //name = area.Name + "_" + name;
            area.SetItem<tt>(name, obj, lSS, tags);
            //if (area.Area < CacheArea.Temp)
            //{
            //    SetItem(CacheArea.Temp, name + "_Request_TempCache", obj);
            //}
        }


        #region OverloadAreaSelect

        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(IDataSource area, string name, Func<Task<tt>> createMethod, double lifeSpanSeconds, string tags = "")
        {
            var ca = new DataCache(area);
            return await GetItemAsync<tt>(ca, name, createMethod, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static tt GetItem<tt>(IDataSource area, string name, Func<tt> createMethod, double lifeSpanSeconds, string tags = "")
        {
            var ca = new DataCache(area);
            return GetItem<tt>(ca, name, createMethod, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, Func<Task<tt>> createMethod, double lifeSpanSeconds, string tags = "")
        {
            var ca = CacheSystem.Instance.GetCacheArea(area);
            return await GetItemAsync(ca, name, createMethod, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static tt GetItem<tt>(CacheArea area, string name, Func<tt> createMethod, double lifeSpanSeconds, string tags = "")
        {
            var ca = CacheSystem.Instance.GetCacheArea(area);
            return GetItem(ca, name, createMethod, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static Task<object> GetItemAsync(CacheArea area, string name, Type type, Func<Task<object>> createMethod, double lifeSpanSeconds, string tags = "")
        {
            var ca = CacheSystem.Instance.GetCacheArea(area);
            try
            {

                double? lSS = lifeSpanSeconds;
                if (lSS == 0)
                {
                    lSS = null;
                }
                if (!name.Contains("CacheEnabled") && !CacheSystem.Instance.CacheEnabled)
                {
                    return createMethod?.Invoke();
                }
                if (ca != null)
                {
                    name = ca.Name + "_" + name;
                    return ca.GetItemAsync(name, createMethod, lSS, tags);
                }
                if (createMethod != null)
                {
                    return createMethod();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error:" + ex.Message + " retrieving cache item [" + name + "] on cache [" + area.ToString() + "]", ex);
            }
            return null;
            //return await GetItemAsync(ca, name, createMethod, lifeSpanSeconds);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static object GetItem(CacheArea area, string name, Type type, Func<object> createMethod, double lifeSpanSeconds, string tags = "")
        {
            var ca = CacheSystem.Instance.GetCacheArea(area);
            return GetItem(ca, name, createMethod, lifeSpanSeconds, tags);
        }

        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static async Task SetItemAsync<tt>(IDataSource area, string name, tt obj, double lifeSpanSeconds, string tags = "")
        {
            var ca = new DataCache(area);
            await SetItemAsync<tt>(ca, name, obj, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static void SetItem<tt>(IDataSource area, string name, tt obj, double lifeSpanSeconds, string tags = "")
        {
            var ca = new DataCache(area);
            SetItem<tt>(ca, name, obj, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static async Task SetItemAsync<tt>(CacheArea area, string name, tt obj, double lifeSpanSeconds, string tags = "")
        {
            var ca = CacheSystem.Instance.GetCacheArea(area);
            await SetItemAsync(ca, name, obj, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static async Task SetItemAsync(CacheArea area, string name, Type type, object obj, double lifeSpanSeconds, string tags = "")
        {
            var ca = CacheSystem.Instance.GetCacheArea(area);
            await SetItemAsync(ca, name, obj, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static void SetItem<tt>(CacheArea area, string name, tt obj, double lifeSpanSeconds, string tags = "")
        {
            var ca = CacheSystem.Instance.GetCacheArea(area);
            SetItem(ca, name, obj, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static void SetItem(CacheArea area, string name, Type type, object obj, double lifeSpanSeconds, string tags = "")
        {
            var ca = CacheSystem.Instance.GetCacheArea(area);
            SetItem(ca, name, obj, lifeSpanSeconds, tags);
        }


        #endregion

        #region OverloadedFunctions

        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, Func<tt> createMethod, double lifeSpanSeconds, string tags = "")
        {
            return await GetItemAsync<tt>(area, name, async () => createMethod(), lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static Task<object> GetItemAsync(CacheArea area, string name, Type type, Func<object> createMethod, double lifeSpanSeconds, string tags = "")
        {
            return GetItemAsync(area, name, type, async () => createMethod(), lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, Func<tt> createMethod, string tags = "")
        {
            return await GetItemAsync<tt>(area, name, async () => createMethod(), 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static Task<object> GetItemAsync(CacheArea area, string name, Type type, Func<object> createMethod, string tags = "")
        {
            return GetItemAsync(area, name, type, async () => createMethod(), 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, Func<Task<tt>> createMethod, string tags = "")
        {
            return await GetItemAsync<tt>(area, name, createMethod, 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<object> GetItemAsync(CacheArea area, string name, Type type, Func<Task<object>> createMethod, string tags = "")
        {
            //await Task.Delay(10000);

            return await GetItemAsync(area, name, type, createMethod, 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, string tags = "")
        {
            return await GetItemAsync<tt>(area, name, async ()=> default(tt), 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <returns></returns>
        public static Task<object> GetItemAsync(CacheArea area, string name, Type type, string tags = "")
        {
            return GetItemAsync(area, name, type, async () => null, 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, double lifeSpanSeconds, string tags = "")
        {
            return await GetItemAsync<tt>(area, name, async () => default(tt), lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static Task<object> GetItemAsync(CacheArea area, string name, Type type, double lifeSpanSeconds, string tags = "")
        {
            return GetItemAsync(area, name, type, async () => null, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, tt defaultValue, string tags = "")
        {
            return await GetItemAsync<tt>(area, name, async () => defaultValue, 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <returns></returns>
        public static Task<object> GetItemAsync(CacheArea area, string name, Type type, object defaultValue, string tags = "")
        {
            return GetItemAsync(area, name, type, async () => defaultValue, 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, tt defaultValue, double lifeSpanSeconds, string tags = "")
        {
            return await GetItemAsync<tt>(area, name, async () => defaultValue, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static Task<object> GetItemAsync(CacheArea area, string name, Type type, object defaultValue, double lifeSpanSeconds, string tags = "")
        {
            return GetItemAsync(area, name, type, async () => defaultValue, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(IDataSource area, string name, Func<Task<tt>> createMethod, string tags = "")
        {
            return await GetItemAsync<tt>(area, name, createMethod, 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(IDataSource area, string name, string tags = "")
        {
            return await GetItemAsync<tt>(area, name, async () => default(tt), 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(IDataSource area, string name, double lifeSpanSeconds, string tags = "")
        {
            return await GetItemAsync<tt>(area, name, async () => default(tt), lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(IDataSource area, string name, tt defaultValue, string tags = "")
        {
            return await GetItemAsync<tt>(area, name, async () => defaultValue, 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(IDataSource area, string name, tt defaultValue, double lifeSpanSeconds, string tags = "")
        {
            return await GetItemAsync<tt>(area, name, async () => defaultValue, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static tt GetItem<tt>(CacheArea area, string name, Func<tt> createMethod, string tags = "")
        {
            return GetItem<tt>(area, name, createMethod, 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static object GetItem(CacheArea area, string name, Type type, Func<object> createMethod, string tags = "")
        {
            return GetItem(area, name, type, createMethod, 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <returns></returns>
        public static tt GetItem<tt>(CacheArea area, string name, string tags = "")
        {
            return GetItem<tt>(area, name, () => default(tt), 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <returns></returns>
        public static object GetItem(CacheArea area, string name, Type type, string tags = "")
        {
            return GetItem(area, name, type, () => null, 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static tt GetItem<tt>(CacheArea area, string name, double lifeSpanSeconds, string tags = "")
        {
            return GetItem<tt>(area, name, () => default(tt), lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static object GetItem(CacheArea area, string name, Type type, double lifeSpanSeconds, string tags = "")
        {
            return GetItem(area, name, type, () => null, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <returns></returns>
        public static tt GetItem<tt>(CacheArea area, string name, tt defaultValue, string tags = "")
        {
            return  GetItem<tt>(area, name, () => defaultValue, 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <returns></returns>
        public static object GetItem(CacheArea area, string name, Type type, object defaultValue, string tags = "")
        {
            return  GetItem(area, name, type,  () => defaultValue, 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static tt GetItem<tt>(CacheArea area, string name, tt defaultValue, double lifeSpanSeconds, string tags = "")
        {
            return  GetItem<tt>(area, name,  () => defaultValue, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static object GetItem(CacheArea area, string name, Type type, object defaultValue, double lifeSpanSeconds, string tags = "")
        {
            return  GetItem(area, name, type,  () => defaultValue, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static tt GetItem<tt>(IDataSource area, string name, Func<tt> createMethod, string tags = "")
        {
            return GetItem<tt>(area, name, createMethod, 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <returns></returns>
        public static tt GetItem<tt>(IDataSource area, string name, string tags = "")
        {
            return  GetItem<tt>(area, name,  () => default(tt), 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static tt GetItem<tt>(IDataSource area, string name, double lifeSpanSeconds, string tags = "")
        {
            return  GetItem<tt>(area, name,  () => default(tt), lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <returns></returns>
        public static tt GetItem<tt>(IDataSource area, string name, tt defaultValue, string tags = "")
        {
            return  GetItem<tt>(area, name,  () => defaultValue, 0, tags);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static tt GetItem<tt>(IDataSource area, string name, tt defaultValue, double lifeSpanSeconds, string tags = "")
        {
            return  GetItem<tt>(area, name,  () => defaultValue, lifeSpanSeconds, tags);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        public static async Task SetItemAsync<tt>(CacheArea area, string name, tt obj, string tags = "")
        {
            await SetItemAsync(area, name, obj, 0, tags);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        public static async Task SetItemAsync(CacheArea area, string name, Type type, object obj, string tags = "")
        {

            await Task.Delay(10000);
            await SetItemAsync(area, name, type, obj, 0, tags);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        public static async Task SetItemAsync<tt>(IDataSource area, string name, tt obj, string tags = "")
        {
            await SetItemAsync(area, name, obj, 0, tags);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        public static void SetItem<tt>(CacheArea area, string name, tt obj, string tags = "")
        {

            SetItem(area, name, obj, 0, tags);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        public static void SetItem(CacheArea area, string name, Type type, object obj, string tags = "")
        {

            SetItem(area, name, type, obj, 0, tags);

        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        public static void SetItem<tt>(IDataSource area, string name, tt obj, string tags = "")
        {
            SetItem<tt>(area, name, obj, 0, tags);
        }

        #endregion
    }
}
