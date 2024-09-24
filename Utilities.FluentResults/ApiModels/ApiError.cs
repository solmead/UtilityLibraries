using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.FluentResults.ApiModels
{
    public class ApiError
    {
        public string ErrorType { get; set; }
        public ApiErrorDetail Detail { get; set; }
    }
}
