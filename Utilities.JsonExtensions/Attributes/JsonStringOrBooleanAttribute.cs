using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utilities.JsonExtensions.Converters;

namespace Utilities.JsonExtensions.Attributes
{
    public class JsonStringOrBooleanAttribute : JsonConverterAttribute
    {
        public JsonStringOrBooleanAttribute() : base(typeof(AutoStringToBooleanConverter))
        {

        }
    }
}
