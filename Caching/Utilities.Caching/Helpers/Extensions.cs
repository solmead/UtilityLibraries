using System.Collections.Generic;
using System.Linq;
using Utilities.Caching.Core.Collections.List;

namespace Utilities.Caching.Helpers
{
    public static class Extensions
    {

        public static bool ContainsKey(this List<CachedEntryBase> dictionary, string name)
        {
            return (from ce in dictionary where ce.Name.ToUpper() == name select ce).Any();
        }
        public static CachedEntryBase getByName(this List<CachedEntryBase> dictionary, string name)
        {
            return (from ce in dictionary where ce.Name.ToUpper() == name select ce).FirstOrDefault();
        }
        
    }
}
