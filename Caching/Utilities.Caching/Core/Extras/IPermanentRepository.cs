using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utilities.Caching.Core.Extras
{
    public interface IPermanentRepository
    {

        Task<byte[]> GetAsync(string name);
        Task SetAsync(string name, byte[] value, TimeSpan? timeout);
        Task DeleteAsync(string name);
        Task<List<string>> GetKeysAsync();


        byte[] Get(string name);
        void Set(string name, byte[] value, TimeSpan? timeout);
        void Delete(string name);
        List<string> GetKeys();


    }
}
