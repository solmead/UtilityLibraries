using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Caching.Core.Extras
{
    public class StoreNowhere :IPermanentRepository
    {

        public async Task<byte[]> GetAsync(string name)
        {
            return null;
        }

        public async Task SetAsync(string name, byte[] value, TimeSpan? timeout)
        {
            
        }

        public async Task DeleteAsync(string name)
        {
            
        }

        public async Task<List<string>> GetKeysAsync()
        {
            return new List<string>();
        }

        public byte[] Get(string name)
        {
            return null;
        }

        public void Set(string name, byte[] value, TimeSpan? timeout)
        {
            
        }

        public void Delete(string name)
        {
            
        }

        public List<string> GetKeys()
        {
            return new List<string>();
        }
    }
}
