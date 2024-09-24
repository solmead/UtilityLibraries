using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Utilities.JsonExtensions.Converters
{
    public class AutoStringOrDateTimeConverter : JsonConverter<DateTime?>
    {
        public AutoStringOrDateTimeConverter() : this(true) { }
        public AutoStringOrDateTimeConverter(bool canWrite) => CanWrite = canWrite;

        public bool CanWrite { get; }
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.String:
                    var s = reader.GetString();
                    return null;
                default:
                    DateTime value;
                    reader.TryGetDateTime(out value);
                    return value;
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
