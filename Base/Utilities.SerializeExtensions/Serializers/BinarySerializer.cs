using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Utilities.SerializeExtensions.Serializers
{
    public class BinarySerializer : ISerializer
    {

        public Action<string> LogMessage { get; set; }
        public Encoding BaseEncoding { get; set; } = Encoding.Unicode;

        private void Log(string msg)
        {
            LogMessage?.Invoke(msg);
        }

        public byte[] SerializeToArray<T>(T item)
        {
            return SerializeToArray(item, typeof(T));
        }
        public string Serialize<T>(T item)
        {
            return Serialize(item, typeof(T)); 
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

        public T Deserialize<T>(string data)
        {
            return (T)Deserialize(data, typeof(T));
        }
        public T Deserialize<T>(byte[] data)
        {
            return (T)Deserialize(data, typeof(T));
        }
        public object Deserialize(string data, Type type)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;

            var obj = Deserialize(Convert.FromBase64String(data), type);
            return obj;
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
