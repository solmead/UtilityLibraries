using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Caching.Database.Configuration
{
    public static class Configurator
    {
        public static string ConnString;

        public static void ConfigureDatabaseCache(this IServiceCollection services)
        {
            //Utilities.Caching.Configuration.Configurator.ConfigureCache(services);
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }


        public static void InitDatabaseCache(string connectionString) //this IApplicationBuilder app)
        {

             ConnString = connectionString;
            var system = Cache.Instance;
            system.CacheAreas[CacheArea.Permanent] = new DatabaseCache(connectionString);

            //var serviceProvider = app.ApplicationServices;
            //var contextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        }

    }
}
