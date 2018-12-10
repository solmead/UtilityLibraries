using System;
using System.Text;
using Newtonsoft.Json;

namespace Utilities.SerializeExtensions.Serializers
{
    public class JsonSerializer : ISerializer
    {
        public T Deserialize<T>(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return default(T);

            return JsonConvert.DeserializeObject<T>(data);
        }

        public object Deserialize(string data, Type type)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;

            return JsonConvert.DeserializeObject(data, type);
        }

        public T Deserialize<T>(byte[] data)
        {
            var s = Encoding.UTF8.GetString(data);
            return Deserialize<T>(s);
        }

        public object Deserialize(byte[] data, Type type)
        {
            var s = Encoding.UTF8.GetString(data);
            return Deserialize(s, type);
        }

        public string Serialize<T>(T item)
        {
            return JsonConvert.SerializeObject(item);
        }

        public string Serialize(object item, Type type)
        {
            return JsonConvert.SerializeObject(item);
        }

        public byte[] SerializeToArray<T>(T item)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
        }

        public byte[] SerializeToArray(object item, Type type)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
        }
    }
}
