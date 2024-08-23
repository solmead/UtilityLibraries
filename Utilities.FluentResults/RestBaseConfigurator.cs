using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utilities.KeyValueStore;

namespace Utilities.FluentResults
{
    public static class RestBaseConfigurator
    {
        internal static TItem? Create<TItem>()
        {
            var tp = typeof(TItem);
            var newItem = (TItem?)tp.Assembly.CreateInstance(tp.FullName);
            return newItem;
        }

        public static IServiceCollection InitilizeRestServices<TTSettings>(this IServiceCollection services, ILogger logger, IHostEnvironment env, ISettingsRepository config, TTSettings? defaultSettings = null)
            where TTSettings :RestSettings
        {
            var settings = defaultSettings ?? Create<TTSettings>();

            var x = config.GetValueSeperate<TTSettings>(settings?.ClientName + "Settings", settings);

            services.AddSingleton(x);

            services.AddHttpClient(x.ClientName, c =>
            {
                c.BaseAddress = new Uri(x.ApiAddress);
                //string base64EncodedCredentials = Base64Encode(String.Format("{0}:{1}", x.RojoUsername, x.RojoPassword));
                //c.DefaultRequestHeaders.Add($"Authorization", $"Basic {base64EncodedCredentials}");
                c.DefaultRequestHeaders.Add("Accept-Encoding", "gzip"); //This can be changed to another compression method, gzip was just arbitrarily selected.
                c.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", x.SubscriptionKey);

            }).ConfigurePrimaryHttpMessageHandler(() => new LoggingHandler(new HttpClientHandler
            {
                //Added so that responses are decompressed upon recieving.  This can be removed if we remove the Accept-Encoding gzip above.  
                AutomaticDecompression = DecompressionMethods.GZip
            }))
            .AddPolicyHandler(CatalystDataPollyPolicy(logger))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(5));




            return services;
        }

        private static AsyncRetryPolicy<HttpResponseMessage> CatalystDataPollyPolicy(ILogger logger)
        {
            // Handle both exceptions and return values in one policy
            HttpStatusCode[] httpStatusCodesWorthRetrying = {
               HttpStatusCode.RequestTimeout, // 408
               HttpStatusCode.InternalServerError, // 500
               HttpStatusCode.BadGateway, // 502
               //HttpStatusCode.ServiceUnavailable, // 503
               HttpStatusCode.GatewayTimeout // 504
            };

            var policy = Policy.Handle<HttpRequestException>()
                .Or<TimeoutRejectedException>()
                .OrResult<HttpResponseMessage>(r =>
                {
                    logger.LogError($"OnResult Running for httpStatusCode: {r.StatusCode}!");
                    return httpStatusCodesWorthRetrying.Contains(r.StatusCode);
                })
                .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogError($"Wait and Retry Occuring: timeSpan: {timeSpan}, currentRetryCount {retryCount}");
                });

            return policy;

        }


    }
}
