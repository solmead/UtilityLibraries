using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Caching.Database.Context;

namespace Utilities.Caching.Database
{
    public static class Core
    {



        public static void Setup(string connectionString)
        {

            var system = CacheSystem.Instance;
            system.CacheAreas[CacheArea.Permanent] = new DatabaseCache(connectionString);
        }
    }
}
