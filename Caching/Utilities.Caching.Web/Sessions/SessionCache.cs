using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Caching.Core;

namespace Utilities.Caching.Web.Sessions
{
    public class SessionCache : DataCache
    {
        public SessionCache()
            : base(new SessionDataSource())
        {
            Area = CacheArea.Session;
            Name = "StandardSession";
        }
    }
}
