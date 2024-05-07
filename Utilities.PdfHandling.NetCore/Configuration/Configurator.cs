using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Utilities.FileExtensions.Services;
using Utilities.PdfHandling.NetCore.Abstracts;
using Utilities.PdfHandling.NetCore.Concretes;

namespace Utilities.PdfHandling.NetCore.Configuration
{
    public static class Configurator
    {
        public static IServiceCollection InitilizePdfHandling(this IServiceCollection services, IHostEnvironment env, Action<PdfConfig> setupAction)
        {
            Core.config = Core.config ?? new PdfConfig();

            services.AddTransient<IPdfServicesClient, PdfServicesClient>();
            services.AddTransient<IPdfCreation, PdfCreation>();

            Core.config.CurrentServer = Enum.TryParse(env.EnvironmentName, out ServerEnum result) ? result : ServerEnum.Development;

            if (setupAction != null)
            {
                setupAction(Core.config);
            }

            services.AddHttpClient("PdfServicesDevelopmentClient", c =>
            {
                c.BaseAddress = new Uri("https://webservices-webdev2.uc.edu/pdfservices/");
                c.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                //Added so that responses are decompressed upon recieving.  This can be removed if we remove the Accept-Encoding gzip above.  
                AutomaticDecompression = DecompressionMethods.GZip
            })
            .AddPolicyHandler(DataPollyPolicy())
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(30));


            services.AddHttpClient("PdfServicesQAClient", c =>
            {
                c.BaseAddress = new Uri("https://webservices-webqa2.uc.edu/pdfservices/");
                c.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                //Added so that responses are decompressed upon recieving.  This can be removed if we remove the Accept-Encoding gzip above.  
                AutomaticDecompression = DecompressionMethods.GZip
            })
            .AddPolicyHandler(DataPollyPolicy())
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(30));


            services.AddHttpClient("PdfServicesScanClient", c =>
            {
                c.BaseAddress = new Uri("https://webservices-scan2.uc.edu/pdfservices/");
                c.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                //Added so that responses are decompressed upon recieving.  This can be removed if we remove the Accept-Encoding gzip above.  
                AutomaticDecompression = DecompressionMethods.GZip
            })
            .AddPolicyHandler(DataPollyPolicy())
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(30));


            services.AddHttpClient("PdfServicesProductionClient", c =>
            {
                c.BaseAddress = new Uri("https://webservices-ext.uc.edu/pdfservices/");
                c.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                //Added so that responses are decompressed upon recieving.  This can be removed if we remove the Accept-Encoding gzip above.  
                AutomaticDecompression = DecompressionMethods.GZip
            })
            .AddPolicyHandler(DataPollyPolicy())
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(30));

            return services;
        }


        private static AsyncRetryPolicy<HttpResponseMessage> DataPollyPolicy()
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
                    //_logger.LogError($"OnResult Running for httpStatusCode: {r.StatusCode}!");
                    return httpStatusCodesWorthRetrying.Contains(r.StatusCode);
                })
                .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    //_logger.LogError($"Wait and Retry Occuring: timeSpan: {timeSpan}, currentRetryCount {retryCount}");
                });

            return policy;

        }
    }
}
