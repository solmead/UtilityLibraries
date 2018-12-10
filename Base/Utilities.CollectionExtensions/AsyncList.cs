using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Collections.Extensions
{
    [Serializable]
    public class AsyncList<TT> : IAsyncList<TT>
    {
        public IList<TT> _baseList;

        public AsyncList()
        {
            _baseList = new List<TT>();
        }
        public AsyncList(IList<TT> baseList)
        {
            _baseList = baseList;
        }

        public async Task<IList<TT>> ToListAsync()
        {
            return _baseList;
        }

        public async Task<IEnumerator<TT>> GetEnumeratorAsync()
        {
            return _baseList.GetEnumerator();
        }

        public async Task AddAsync(TT item)
        {
            _baseList.Add(item);
        }

        public async Task ClearAsync()
        {
            _baseList.Clear();
        }

        public async Task<bool> ContainsAsync(TT item)
        {
            return _baseList.Contains(item);
        }

        public async Task CopyToAsync(TT[] array, int arrayIndex)
        {
            _baseList.CopyTo(array, arrayIndex);
        }

        public async Task<bool> RemoveAsync(TT item)
        {
            return _baseList.Remove(item);
        }

        public async Task<int> CountAsync()
        {
            return _baseList.Count;
        }

        public async Task<bool> IsReadOnlyAsync()
        {
            return _baseList.IsReadOnly;
        }

        public async Task<int> IndexOfAsync(TT item)
        {
            return _baseList.IndexOf(item);
        }

        public async Task InsertAsync(int index, TT item)
        {
            _baseList.Insert(index, item);
        }

        public async Task RemoveAtAsync(int index)
        {
            _baseList.RemoveAt(index);
        }

        public async Task<TT> GetAsync(int index)
        {
            return _baseList[index];
        }

        public async Task SetAsync(int index, TT value)
        {
            _baseList[index] = value;
        }
    }
}
