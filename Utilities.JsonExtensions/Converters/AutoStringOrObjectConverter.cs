using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Utilities.JsonExtensions.Converters
{
    public class AutoStringOrObjectConverter<TItem> : JsonConverter<TItem> where TItem : class
    {
        public AutoStringOrObjectConverter() : this(true) { }
        public AutoStringOrObjectConverter(bool canWrite) => CanWrite = canWrite;

        public bool CanWrite { get; }
        public override TItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.String:
                    return null;
                default:
                    return JsonSerializer.Deserialize<TItem>(ref reader, options);
            }
        }

        public override void Write(Utf8JsonWriter writer, TItem value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
