using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.FluentResults.ApiModels
{
    public class ApiErrorDetail
    {

        public ApiErrorDetail(string message, Dictionary<string, object> metadata)
        {
            Message = message;

            //Dictionary<string, string> dString = dObject.ToDictionary(k => k.Key, k => k.Value.ToString());
            Metadata = metadata.ToDictionary(k => k.Key, k => k.Value.ToString());
        }
        public string Message { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

}
