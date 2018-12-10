//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Utilities.Caching.Helpers;

//namespace Utilities.Caching.Core.DataSources
//{
//    public class ThreadDataSource : IDataSource
//    {
//        public CachedEntry<tt> GetItem<tt>(string name)
//        {
//            return AsyncHelper.RunSync(() => GetItemAsync<tt>(name));
//        }

//        public void SetItem<tt>(CachedEntry<tt> item)
//        {
//            AsyncHelper.RunSync(() => SetItemAsync<tt>(item));
//        }

//        public void DeleteItem(string name)
//        {
//            AsyncHelper.RunSync(() => DeleteItemAsync(name));
//        }

//        public void DeleteAll()
//        {
//            AsyncHelper.RunSync(DeleteAllAsync);
//        }

//        public async Task<CachedEntry<tt>> GetItemAsync<tt>(string name)
//        {
//            object empty = default(tt);
//            tt tObj = default(tt);
//            var entry = Thread.GetData(Thread.GetNamedDataSlot(name.ToUpper())) as CachedEntry<tt>;
//            if (entry == null || (entry.TimeOut.HasValue && entry.TimeOut.Value < DateTime.Now))
//            {
//                entry = null;
//                await DeleteItemAsync(name);
//            }
//            if (entry != null)
//            {
//                try
//                {
//                    tObj = (tt) (entry.Item);
//                }
//                catch (Exception)
//                {
//                    entry = null;
//                    DeleteItem(name);
//                }
//            }
//            return entry;
//        }

//        public async Task SetItemAsync<tt>(CachedEntry<tt> item)
//        {

//            Thread.SetData(Thread.GetNamedDataSlot(item.Name.ToUpper()), item);
//        }

//        public async Task DeleteItemAsync(string name)
//        {
//            Thread.SetData(Thread.GetNamedDataSlot(name.ToUpper()), null);
//        }

//        public async Task DeleteAllAsync()
//        {
//            //throw new NotImplementedException();
//        }
//    }
//}
