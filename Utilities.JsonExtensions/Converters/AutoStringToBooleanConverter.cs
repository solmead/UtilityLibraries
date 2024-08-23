using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Utilities.JsonExtensions.Converters
{
    public class AutoStringToBooleanConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                typeToConvert = typeToConvert.GetGenericArguments()[0];
            }
            // see https://stackoverflow.com/questions/1749966/c-sharp-how-to-determine-whether-a-type-is-a-number
            switch (Type.GetTypeCode(typeToConvert))
            {
                case TypeCode.Boolean:
                    return true;
                default:
                    return false;
            }
        }
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                return bool.TryParse(s, out var i) ?
                    i : throw new Exception($"unable to parse {s} to boolean");
            }
            if (reader.TokenType == JsonTokenType.Number)
            {
                return (reader.TryGetInt64(out long l) ?
                    l :
                    reader.GetDouble()) > 0;
            }

            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                throw new Exception($"unable to parse {document.RootElement.ToString()} to boolean");
            }
        }


        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var str = value.ToString();             // I don't want to write int/decimal/double/...  for each case, so I just convert it to string . You might want to replace it with strong type version.
            if (bool.TryParse(str, out var i))
            {
                writer.WriteBooleanValue(i);
            }
            else
            {
                throw new Exception($"unable to parse {str} to number");
            }
        }
    }

}
