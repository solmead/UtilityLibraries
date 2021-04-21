using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utilities.Caching.Core.Extras
{
    public interface IPermanentRepository
    {

        Task<byte[]> GetBytesAsync(string name);
        Task<string> GetStringAsync(string name);
        Task SetAsync(string name, string value, TimeSpan? timeout);
        Task SetAsync(string name, byte[] value, TimeSpan? timeout);
        Task DeleteAsync(string name);
        Task<List<string>> GetKeysAsync();


        byte[] GetBytes(string name);
        string GetString(string name);
        void Set(string name, string value, TimeSpan? timeout);
        void Set(string name, byte[] value, TimeSpan? timeout);
        void Delete(string name);
        List<string> GetKeys();


    }
}
