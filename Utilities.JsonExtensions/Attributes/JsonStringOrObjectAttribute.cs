using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utilities.JsonExtensions.Converters;

namespace Utilities.JsonExtensions.Attributes
{
    public class JsonStringOrObjectAttribute<TT> : JsonConverterAttribute
        where TT: class
    {
        public JsonStringOrObjectAttribute() : base(typeof(AutoStringOrObjectConverter<TT>))
        {

        }
    }
}
