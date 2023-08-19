using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Caching.Core;

namespace Utilities.Caching.AspNetCore.Sessions
{
    public class SessionCache : DataCache
    {
        public SessionCache(IHttpContextAccessor contextAccessor, ILogger logger)
            : base(new SessionDataSource(contextAccessor, logger))
        {
            Area = CacheArea.Session;
            Name = "StandardSession";
        }
    }
}
