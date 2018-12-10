using Microsoft.AspNetCore.Http;
using Utilities.Caching.Core;
using Utilities.Caching.Core.DataSources;

namespace Utilities.Caching.AspNetCore
{
    public class RequestCache : DataCache
    {
        public RequestCache(IHttpContextAccessor contextAccessor)
            : base(new RequestDataSource(contextAccessor))
        {
            Area = CacheArea.Request;
            Name = "DefaultRequest";
        }
    }
}
