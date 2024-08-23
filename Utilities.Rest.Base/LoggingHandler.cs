using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Rest.Base
{
    internal class LoggingHandler : DelegatingHandler
    {

        public static Action<string>? WriteLine { get; set; } = null;

        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (WriteLine == null)
            {
                WriteLine = (msg) => Console.WriteLine(msg);
            }

            WriteLine("Request:");
            WriteLine(request.ToString());
            if (request.Content != null)
            {
                WriteLine(await request.Content.ReadAsStringAsync());
            }
            WriteLine("");

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            WriteLine("Response:");
            WriteLine(response.ToString());
            if (response.Content != null)
            {
                WriteLine(await response.Content.ReadAsStringAsync());
            }
            WriteLine("");

            return response;
        }
    }
}
