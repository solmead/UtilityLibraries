using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Utilities.SerializeExtensions.Serializers
{
    public class BinarySerializer : ISerializer
    {
        

        public byte[] SerializeToArray<T>(T item)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, item);
                stream.Flush();
                stream.Position = 0;
                return stream.ToArray();
            }
        }
        public string Serialize<T>(T item)
        {
            return Convert.ToBase64String(SerializeToArray<T>(item));
        }
        
        public string Serialize(object item, Type type)
        {
            return Convert.ToBase64String(SerializeToArray(item, type));
        }

        public byte[] SerializeToArray(object item, Type type)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, item);
                stream.Flush();
                stream.Position = 0;
                return stream.ToArray();
            }
        }


        public T Deserialize<T>(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return default(T);
            byte[] b = Convert.FromBase64String(base64String);

            return Deserialize<T>(b);
        }

        public T Deserialize<T>(byte[] data)
        {
            if (data == null || data.Length == 0)
                return default(T);
            using (var stream = new MemoryStream(data))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public object Deserialize(string data, Type type)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;
            byte[] b = Convert.FromBase64String(data);

            return Deserialize(b, type);
        }

        public object Deserialize(byte[] data, Type type)
        {
            if (data == null || data.Length == 0)
                return null;
            using (var stream = new MemoryStream(data))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(stream);
            }
        }
    }
}
