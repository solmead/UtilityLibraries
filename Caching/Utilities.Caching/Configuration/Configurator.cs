using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Caching.Core;
using Utilities.Caching.Helpers;
using Utilities.SerializeExtensions;
using Utilities.SerializeExtensions.Serializers;

namespace Utilities.Caching.Configuration
{
    public static class Configurator
    {

        private static CacheSystem __localInstance;

        private static object CacheSystemCreateLock = new object();

        private static IServiceProvider _serviceProvider { get; set; }


        public static ICacheCookie CacheCookie { get; set; }

        private static ILogger _logger { get; set; }


        public static CacheSystem Instance
        {
            get
            {
                if (_serviceProvider == null && _logger == null)
                {
                    throw new Exception("Must Initialize Cache before use. Call Utilities.Caching.Configuration.Configurator.InitCache");
                }
                CacheSystem _instance = null;
                if (_serviceProvider != null)
                {
                    _instance = _serviceProvider.GetRequiredService<CacheSystem>();
                    _logger = _serviceProvider.GetRequiredService<ILogger>();
                }
                else
                {
                    if (__localInstance != null)
                    {
                        _instance = __localInstance;
                    }
                    else
                    {
                        lock (CacheSystemCreateLock)
                        {
                            if (__localInstance != null)
                            {
                                _instance = __localInstance;
                            }
                            else
                            {

                                __localInstance = new CacheSystem(_logger);
                                _instance = __localInstance;
                            }
                        }
                    }
                }

                if (_instance == null)
                {
                    throw new Exception("Must Configure Cache before use. Call Utilities.Caching.Configuration.Configurator.ConfigureCache");
                }
                return _instance;
            }
        }
        public static ISerializer Serializer
        {
            get => Cache.GetItem<ISerializer>(CacheArea.Global, "CachingSerializer", () => new Serializer(_logger));
            set => Cache.SetItem<ISerializer>(CacheArea.Global, "CachingSerializer", value);
        }
        //public static void Initilize(ITimedTaskRepository timedTaskRepository, ILogger logger)
        //{
        //    logger.LogInformation("Initilizing Timed Tasks Services");
        //    _instance = new TaskServices(timedTaskRepository, logger);
        //}

        public static void ConfigureCache(this IServiceCollection services)
        {
            services.AddSingleton<CacheSystem>();
        }

        public static void SetCookieRepository(ICookieRepository cookieRepository)
        {
            CacheCookie = new CacheCookies(cookieRepository);
        }

        public static void InitCache(ILogger logger)
        {
            _logger = logger;
        }
        public static void InitCache(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }



        //public static void InitCache(this IApplicationBuilder app)
        //{
        //    var serviceProvider = app.ApplicationServices;

        //    _serviceProvider = serviceProvider;
        //}

    }
}
