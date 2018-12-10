using System;
using System.Collections.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Utilities.Caching.CacheAreas;

namespace Utilities.Caching.Core.Collections.List
{
    public class AsyncListOnCache<TT> : IAsyncList<TT>
    {
        private readonly IDataSource _dataSource = null;

        public string Name { get; private set; }
        //public double LifeSpanInSeconds { get; set; }
        public CacheArea TempCacheArea { get; set; }
        public double TempCacheTime { get; set; }

        public IDataSource DataSource { get { return _dataSource; } }

        public AsyncListOnCache(string name, CacheArea cache, CacheArea tempCacheArea = CacheArea.Temp, double tempCacheTime = 0)
        {
            TempCacheArea = tempCacheArea;
            TempCacheTime = tempCacheTime;
            Name = name;
            _dataSource = CacheSystem.Instance.GetCacheArea(cache).DataSource;
            //LifeSpanInSeconds = lifeSpanInSeconds;
        }
        public AsyncListOnCache(string name, ICacheArea cache, CacheArea tempCacheArea = CacheArea.Temp, double tempCacheTime = 0)
        {
            TempCacheArea = tempCacheArea;
            TempCacheTime = tempCacheTime;
            Name = name;
            _dataSource = cache.DataSource;
            //LifeSpanInSeconds = lifeSpanInSeconds;
        }
        public AsyncListOnCache(string name, IDataSource cache, CacheArea tempCacheArea = CacheArea.Temp, double tempCacheTime = 0)
        {
            TempCacheArea = tempCacheArea;
            TempCacheTime = tempCacheTime;
            Name = name;
            _dataSource = cache;
            //LifeSpanInSeconds = lifeSpanInSeconds;
        }

        private void WriteLine(string msg)
        {
            Debug.WriteLine(DateTime.Now.ToLongTimeString() + " : " + DateTime.Now.Millisecond + " - " + msg); 
        }

        private async Task<IList<TT>> BaseListAsync()
        {
            WriteLine("Refreshing cache list base list [" + Name + "]");
                return await _dataSource.GetListAsync<TT>(Name);
        }


        private async Task<IList<TT>> LocalListAsync()
        {
                return await Cache.GetItemAsync<IList<TT>>(TempCacheArea, Name + "_localList", async () => await BaseListAsync(), TempCacheTime);
        }

        private async Task ClearLocalListAsync()
        {
            await Cache.SetItemAsync<List<TT>>(TempCacheArea, Name + "_localList", null);
        }


        public Task<IList<TT>> ToListAsync()
        {
            return LocalListAsync();
        }

        public async Task<IEnumerator<TT>> GetEnumeratorAsync()
        {
            return (await LocalListAsync()).GetEnumerator();
        }


        public async Task AddAsync(TT item)
        {
            (await LocalListAsync()).Add(item);
            await _dataSource.AddToListAsync(Name, item);
        }

        public async Task ClearAsync()
        {
            (await LocalListAsync()).Clear();
            await _dataSource.ClearListAsync<TT>(Name);
        }

        public async Task<bool> ContainsAsync(TT item)
        {
            return (await LocalListAsync()).Contains(item);
        }

        public async Task CopyToAsync(TT[] array, int arrayIndex)
        {
            (await LocalListAsync()).CopyTo(array, arrayIndex);
        }

        public async Task<bool> RemoveAsync(TT item)
        {
            if ((await LocalListAsync()).Contains(item))
            {
                await _dataSource.RemoveFromListAsync(Name, item);
                (await LocalListAsync()).Remove(item);
            }
            //ClearLocalList();
            return true;
        }

        public async Task<int> CountAsync() { return (await LocalListAsync()).Count;  }
        public async Task<bool> IsReadOnlyAsync() {  return false;  }
        public async Task<int> IndexOfAsync(TT item)
        {
            return (await LocalListAsync()).IndexOf(item);
        }

        public async Task InsertAsync(int index, TT item)
        {
            (await LocalListAsync()).Insert(index, item);
            await _dataSource.InsertIntoListAsync(Name, index, item);
        }

        public async Task RemoveAtAsync(int index)
        {
            (await LocalListAsync()).RemoveAt(index);
            await _dataSource.RemoveFromListAtAsync<TT>(Name, index);
        }

        public async Task<TT> GetAsync(int index)
        {
            return (await LocalListAsync())[index];
        }

        public async Task SetAsync(int index, TT value)
        {
            (await LocalListAsync())[index] = value;
            await _dataSource.SetInListAsync(Name, index, value);
        }

        //public async Task<TT> this[int index]
        //{
        //    get { return (await LocalListAsync())[index]; }
        //    set
        //    {
        //        (await LocalListAsync())[index] = value;
        //        _dataSource.SetInList(Name, index, value);
        //    }
        //}
    }
}
