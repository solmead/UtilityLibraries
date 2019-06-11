using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Collections.Extensions
{
    [Serializable]
    public class AsynchronousList<TT> : IAsynchronousList<TT>
    {
        public IList<TT> _baseList;

        public AsynchronousList()
        {
            _baseList = new List<TT>();
        }
        public AsynchronousList(IList<TT> baseList)
        {
            _baseList = baseList;
        }

        public Task<IList<TT>> ToListAsync()
        {
            return Task.FromResult(_baseList);
        }

        public  Task<IEnumerator<TT>> GetEnumeratorAsync()
        {
            return Task.FromResult(_baseList.GetEnumerator());
        }

        public Task AddAsync(TT item)
        {
            _baseList.Add(item);
            return Task.CompletedTask;
        }

        public Task ClearAsync()
        {
            _baseList.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> ContainsAsync(TT item)
        {
            return Task.FromResult( _baseList.Contains(item));
        }

        public Task CopyToAsync(TT[] array, int arrayIndex)
        {
            _baseList.CopyTo(array, arrayIndex);

            return Task.CompletedTask;
        }

        public Task<bool> RemoveAsync(TT item)
        {
            return Task.FromResult( _baseList.Remove(item));
        }

        public Task<int> CountAsync()
        {
            return Task.FromResult(_baseList.Count);
        }

        public Task<bool> IsReadOnlyAsync()
        {
            return Task.FromResult(_baseList.IsReadOnly);
        }

        public Task<int> IndexOfAsync(TT item)
        {
            return Task.FromResult(_baseList.IndexOf(item));
        }

        public Task InsertAsync(int index, TT item)
        {
            _baseList.Insert(index, item);
            return Task.CompletedTask;
        }

        public Task RemoveAtAsync(int index)
        {
            _baseList.RemoveAt(index);
            return Task.CompletedTask;
        }

        public Task<TT> GetAsync(int index)
        {
            return Task.FromResult(_baseList[index]);
        }

        public Task SetAsync(int index, TT value)
        {
            _baseList[index] = value;
            return Task.CompletedTask;
        }
    }
}
