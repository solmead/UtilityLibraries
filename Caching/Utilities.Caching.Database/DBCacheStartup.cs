using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Utilities.Caching.Database
{
    public static class DBCacheStartup
    {


        public static void Init(string connectionString)
        {
            Core.Setup(connectionString);


        }
    }
}
