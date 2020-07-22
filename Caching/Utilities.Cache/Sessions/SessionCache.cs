using Microsoft.AspNetCore.Http;
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
        public SessionCache(IHttpContextAccessor contextAccessor)
            : base(new SessionDataSource(contextAccessor))
        {
            Area = CacheArea.Session;
            Name = "StandardSession";
        }
    }
}
