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

    }
}
