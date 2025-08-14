using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Rest.Base
{
    internal class HttpClientLoggingHandler : DelegatingHandler
    {

        public static Action<string>? WriteLine { get; set; } = null;

        public HttpClientLoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        private void LogMessage(string msg)
        {
            if (WriteLine != null)
            {
                WriteLine(msg);
            } 
            //else
            //{
            //    Debug.WriteLine(msg);
            //}
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            LogMessage("Request:");
            LogMessage(request.ToString());
            if (request.Content != null)
            {
                LogMessage(await request.Content.ReadAsStringAsync());
            }
            LogMessage("");

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            LogMessage("Response:");
            LogMessage(response.ToString());
            if (response.Content != null)
            {
                LogMessage(await response.Content.ReadAsStringAsync());
            }
            LogMessage("");

            return response;
        }
    }
}
