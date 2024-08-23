using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utilities.JsonExtensions.Converters;

namespace Utilities.JsonExtensions.Attributes
{
    public class JsonSingleOrArrayAttribute<TCollection, TItem> : JsonConverterAttribute
        where TCollection : class, ICollection<TItem>, new()
    {
        public JsonSingleOrArrayAttribute() : base(typeof(SingleOrArrayConverter<TCollection, TItem>))
        {

        }
    }
    public class JsonSingleOrArrayAttribute<TItem> : JsonConverterAttribute
    {
        public JsonSingleOrArrayAttribute() : base(typeof(SingleOrArrayConverter<TItem>))
        {

        }
    }
}
