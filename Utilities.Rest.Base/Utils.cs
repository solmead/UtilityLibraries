using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using FluentResults;

namespace Utilities.Rest.Base
{
    public enum DataFormatEnum
    {
        [Description("application/json")]
        JsonEncoded,
        [Description("application/x-www-form-urlencoded")]
        FormEncoded
    }

    public static class Utils
    {

        public static async Task<HttpResponseMessage> DeleteAsync(this HttpClient client, string requestUri, HttpContent? data)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(client.BaseAddress + requestUri),
                Content = data
            };
            var response = await client.SendAsync(request);
            return response;
        }
       


    }
}
