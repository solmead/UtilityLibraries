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
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Utilities.KeyValueStore;

namespace Utilities.Rest.Base
{
    public static class RestBaseConfigurator
    {
        internal static bool useLoggingForClients { get; set; }
        internal static TItem? Create<TItem>()
        {
            var tp = typeof(TItem);
            var newItem = (TItem?)tp.Assembly.CreateInstance(tp.FullName);
            return newItem;
        }

        public static IServiceCollection InitilizeRestServices<TTSettings>(this IServiceCollection services, ILogger logger, IHostEnvironment env, ISettingsRepository config, TTSettings? defaultSettings = null)
            where TTSettings : RestSettings
        {
            var settings = defaultSettings ?? Create<TTSettings>();

            var x = config.GetValueSeperate<TTSettings>(settings?.ClientName + "Settings", settings!);

            services.AddSingleton(x);


            useLoggingForClients = config.GetValueBool("UseLoggingForRestClients", true);


            var conClient = services.AddHttpClient(x.ClientName, c =>
            {
                c.BaseAddress = new Uri(x.ApiAddress!);
                //string base64EncodedCredentials = Base64Encode(String.Format("{0}:{1}", x.RojoUsername, x.RojoPassword));
                //c.DefaultRequestHeaders.Add($"Authorization", $"Basic {base64EncodedCredentials}");
                c.DefaultRequestHeaders.Add("Accept-Encoding", "gzip"); //This can be changed to another compression method, gzip was just arbitrarily selected.
                c.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", x.SubscriptionKey);
                
            });

            if (useLoggingForClients)
            {
                conClient = conClient.ConfigurePrimaryHttpMessageHandler(() => new LoggingHandler(new HttpClientHandler
                {
                    //Added so that responses are decompressed upon recieving.  This can be removed if we remove the Accept-Encoding gzip above.  
                    AutomaticDecompression = DecompressionMethods.GZip
                }));
            } else
            {
                conClient = conClient.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    //Added so that responses are decompressed upon recieving.  This can be removed if we remove the Accept-Encoding gzip above.  
                    AutomaticDecompression = DecompressionMethods.GZip
                });
            }


            if (x.UseRetry ?? false)
            {
                conClient = conClient.AddPolicyHandler(CatalystDataPollyPolicy(logger, x));
            }

            if (x.UseTimeout ?? false)
            {
                conClient = conClient.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(x.Timeout ?? 1.0)));
            }


            return services;
        }

        private static  AsyncRetryPolicy<HttpResponseMessage> CatalystDataPollyPolicy(ILogger logger, RestSettings config)
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
                    var err = httpStatusCodesWorthRetrying.Contains(r.StatusCode);
                    if (useLoggingForClients || err)
                    {
                        logger.LogError($"{config.ClientName} - OnResult Running for httpStatusCode: {r.StatusCode}!");
                    }
                    return err;
                })
                .WaitAndRetryAsync(config.MaxRetries ?? 2, retryAttempt => TimeSpan.FromSeconds(config.RetrySeconds(retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogError($"{config.ClientName} - Wait and Retry Occuring: timeSpan: {timeSpan}, currentRetryCount {retryCount}");
                });

            return policy;

        }


    }
}
