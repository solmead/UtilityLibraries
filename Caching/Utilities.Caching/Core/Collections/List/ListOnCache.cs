using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Utilities.Caching.CacheAreas;
using Generic = System.Collections.Generic;

namespace Utilities.Caching.Core.Collections.List
{
    public class ListOnCache<TT> : IList<TT>
    {
        private readonly IDataSource _dataSource = null;

        public string Name { get; private set; }
        //public double LifeSpanInSeconds { get; set; }
        public CacheArea TempCacheArea { get; set; }
        public double TempCacheTime { get; set; }

        public IDataSource DataSource { get { return _dataSource; } }

        public ListOnCache(string name, CacheArea cache, CacheArea tempCacheArea = CacheArea.Temp, double tempCacheTime = 0)
        {
            TempCacheArea = tempCacheArea;
            TempCacheTime = tempCacheTime;
            Name = name;
            _dataSource = CacheSystem.Instance.GetCacheArea(cache).DataSource;
            //LifeSpanInSeconds = lifeSpanInSeconds;
        }
        public ListOnCache(string name, ICacheArea cache, CacheArea tempCacheArea = CacheArea.Temp, double tempCacheTime = 0)
        {
            TempCacheArea = tempCacheArea;
            TempCacheTime = tempCacheTime;
            Name = name;
            _dataSource = cache.DataSource;
            //LifeSpanInSeconds = lifeSpanInSeconds;
        }
        public ListOnCache(string name, IDataSource cache, CacheArea tempCacheArea = CacheArea.Temp, double tempCacheTime = 0)
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

        private Generic.List<TT> BaseList()
        {
            WriteLine("Refreshing cache list base list [" + Name + "]");
                return _dataSource.GetList<TT>(Name);
        }


        private Generic.List<TT> LocalList
        {
            get
            {
                //WriteLine("Getting cache list local list [" + Name + "] - " + TempCacheArea.ToString() + ":" + TempCacheTime);
                return Cache.GetItem<Generic.List<TT>>(TempCacheArea, Name + "_localList", BaseList, TempCacheTime);
            }
            
        }

        private void ClearLocalList()
        {
            Cache.SetItem<List<TT>>(TempCacheArea, Name + "_localList", null);
        }


        public IEnumerator<TT> GetEnumerator()
        {
            return LocalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TT item)
        {
            LocalList.Add(item);
            _dataSource.AddToList(Name, item);
        }

        public void Clear()
        {
            LocalList.Clear();
            _dataSource.ClearList<TT>(Name);
        }

        public bool Contains(TT item)
        {
            return LocalList.Contains(item);
        }

        public void CopyTo(TT[] array, int arrayIndex)
        {
            LocalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(TT item)
        {
            if (LocalList.Contains(item))
            {
                _dataSource.RemoveFromList(Name, item);
                LocalList.Remove(item);
            }
            //ClearLocalList();
            return true;
        }

        public int Count { get { return LocalList.Count; }  }
        public bool IsReadOnly { get { return false; }  }
        public int IndexOf(TT item)
        {
            return LocalList.IndexOf(item);
        }

        public void Insert(int index, TT item)
        {
            LocalList.Insert(index,item);
            _dataSource.InsertIntoList(Name, index, item);
        }

        public void RemoveAt(int index)
        {
            LocalList.RemoveAt(index);
            _dataSource.RemoveFromListAt<TT>(Name, index);
        }

        public TT this[int index]
        {
            get { return LocalList[index]; }
            set
            {
                LocalList[index] = value;
                _dataSource.SetInList(Name, index, value);
            }
        }
    }
}
