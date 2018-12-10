using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Caching.Helpers;

namespace Utilities.Caching.Core
{
    public interface IDataSource
    {
        BaseCacheArea Area { get; }


        CachedEntry<tt> GetItem<tt>(string name);
        void SetItem<tt>(CachedEntry<tt> item);
        //CachedEntry<object> GetItem(string name, Type type);
        //void SetItem(Type type, CachedEntry<object> item);
        void DeleteItem(string name);
        void DeleteAll();

        Task<CachedEntry<tt>> GetItemAsync<tt>(string name);
        Task SetItemAsync<tt>(CachedEntry<tt> item);
        //Task<CachedEntry<object>> GetItemAsync(string name, Type type);
        //Task SetItemAsync(Type type, CachedEntry<object> item);
        Task DeleteItemAsync(string name);
        Task DeleteAllAsync();



        List<tt> GetList<tt>(string name);
        void AddToList<tt>(string name, tt item);
        void ClearList<tt>(string name);
        void RemoveFromList<tt>(string name, tt item);
        void RemoveFromListAt<tt>(string name, int index);
        void InsertIntoList<tt>(string name, int index, tt item);
        void SetInList<tt>(string name, int index, tt item);



        Task<List<tt>> GetListAsync<tt>(string name);
        Task AddToListAsync<tt>(string name, tt item);
        Task ClearListAsync<tt>(string name);
        Task RemoveFromListAsync<tt>(string name, tt item);
        Task RemoveFromListAtAsync<tt>(string name, int index);
        Task InsertIntoListAsync<tt>(string name, int index, tt item);
        Task SetInListAsync<tt>(string name, int index, tt item);

    }
}
