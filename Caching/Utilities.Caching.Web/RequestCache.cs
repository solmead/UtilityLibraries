using Utilities.Caching.Core;
using Utilities.Caching.Core.DataSources;

namespace Utilities.Caching.Web
{
    public class RequestCache : DataCache
    {
        public RequestCache()
            : base(new RequestDataSource())
        {
            Area = CacheArea.Request;
            Name = "DefaultRequest";
        }
    }
}
