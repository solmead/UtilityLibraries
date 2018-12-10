using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Utilities.Caching.Database
{
    public static class Startup
    {


        public static void InitDatabaseCache(string connectionString)
        {
            Core.Setup(connectionString);


        }
    }
}
