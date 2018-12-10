using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Caching.CacheAreas
{
    public interface INameValueLister
    {
        //IDictionary<string, object> DataDictionary { get;  }
        IDictionary<string, object> DataDictionaryGet();
        Task<IDictionary<string, object>> DataDictionaryGetAsync();
    }
}
