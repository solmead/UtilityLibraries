using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Utilities.SerializeExtensions.Serializers
{
    public class JsonSerializer : ISerializer
    {

        public Action<string> LogMessage { get; set; }

        private void Log(string msg)
        {
            LogMessage?.Invoke(msg);
        }

        public Encoding BaseEncoding { get; set; } = Encoding.Unicode;

        public T Deserialize<T>(string data) where T : class
        {
            return (T)Deserialize(data, typeof(T));
        }
        public T Deserialize<T>(byte[] data) where T : class
        {
            return (T)Deserialize(data, typeof(T));
        }
        public object Deserialize(byte[] data, Type type)
        {

            object obj = null;
            //baseEncoding
            var encoding = BaseEncoding;
            var s = encoding.GetString(data);
            obj = Deserialize(s, type);
            //var s = System.Text.Encoding.UTF8.GetString(data);
            return obj;
        }

        public object Deserialize(string data, Type type)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;

            try
            {
                return JsonConvert.DeserializeObject(data, type);
            }
            catch (Exception)
            {
                return null;
            }
            
        }





        public string Serialize<T>(T item) where T : class
        {
            return Serialize(item, typeof(T));
        }

        public string Serialize(object item, Type type)
        {
            return JsonConvert.SerializeObject(item);
        }

        public byte[] SerializeToArray<T>(T item) where T : class
        {
            return SerializeToArray(item, typeof(T));
        }




        public byte[] SerializeToArray(object item, Type type)
        {
            return BaseEncoding.GetBytes(Serialize(item, type));
        }


    }
}
