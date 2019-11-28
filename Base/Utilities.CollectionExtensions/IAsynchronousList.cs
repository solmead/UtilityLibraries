using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Collections.Extensions
{
    public interface IAsynchronousList<TT>
    {

        Task<IList<TT>> ToListAsync();

        Task<IEnumerator<TT>> GetEnumeratorAsync();

        Task AddAsync(TT item);

        Task ClearAsync();

        Task<bool> ContainsAsync(TT item);

        Task CopyToAsync(TT[] array, int arrayIndex);

        Task<bool> RemoveAsync(TT item);

        Task<int> CountAsync();
        Task<bool> IsReadOnlyAsync();
        Task<int> IndexOfAsync(TT item);

        Task InsertAsync(int index, TT item);

        Task RemoveAtAsync(int index);

        //Task<TT> this[int index] { get; set; }
        Task<TT> GetAsync(int index);
        Task SetAsync(int index, TT value);
    }
}
