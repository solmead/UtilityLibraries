using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Utilities.Caching.Database
{
    public static class DBCacheStartup
    {


        [Obsolete("Use Utilities.Caching.Database.Configuration.InitDatabaseCache", true)]
        public static void Init(string connectionString)
        {
            //Core.Setup(connectionString);


        }
    }
}
