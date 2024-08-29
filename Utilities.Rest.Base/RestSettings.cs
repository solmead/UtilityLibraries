using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Rest.Base
{

    public abstract class RestSettings
    {
        public abstract string ClientName { get; }
        public virtual string? ApiAddress { get; set; }
        public string? SubscriptionKey { get; set; }

        public bool? UseTimeout { get; set; } = true;
        public double? Timeout { get; set; } = 1;


        public bool? UseRetry { get; set; } = true;
        public int? MaxRetries { get; set; } = 2;

        public virtual double RetrySeconds(int attemptNumber)
        {
            return Math.Pow(2, attemptNumber);
        }
    }
}
