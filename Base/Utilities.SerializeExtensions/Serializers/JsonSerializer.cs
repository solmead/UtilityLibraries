﻿using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Utilities.SerializeExtensions.Serializers
{
    public class JsonSerializer : ISerializer
    {

        public Encoding BaseEncoding { get; set; } = Encoding.Unicode;



        private readonly ILogger _logger = null;
        public JsonSerializer()
        {
            
        }
        public JsonSerializer(ILogger logger)
        {
            _logger = logger;
        }

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

            if (obj == null)
            {
                encoding = Encoding.Unicode;
                _logger?.LogDebug("Deserialize - Encoding:" + encoding.EncodingName);
                s = encoding.GetString(data);
                obj = Deserialize(s, type);
            }
            if (obj == null)
            {
                _logger?.LogDebug("Deserialize - Encoding:" + encoding.EncodingName);
                encoding = Encoding.UTF32;
                s = encoding.GetString(data);
                obj = Deserialize(s, type);
            }
            if (obj == null)
            {
                _logger?.LogDebug("Deserialize - Encoding:" + encoding.EncodingName);
                encoding = Encoding.ASCII;
                s = encoding.GetString(data);
                obj = Deserialize(s, type);
            }
            if (obj == null)
            {
                _logger?.LogDebug("Deserialize - Encoding:" + encoding.EncodingName);
                encoding = Encoding.UTF8;
                s = encoding.GetString(data);
                obj = Deserialize(s, type);
            }
            if (obj == null)
            {
                _logger?.LogDebug("Deserialize - Encoding:" + encoding.EncodingName);
                encoding = Encoding.UTF7;
                s = encoding.GetString(data);
                obj = Deserialize(s, type);
            }


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
            catch (Exception ex)
            {
                _logger?.LogError(ex, "JSON Deserialize - Exception:" + ex.ToString());
                return null;
            }

        }





        public string Serialize<T>(T item) where T : class
        {
            return Serialize(item, typeof(T));
        }

        public string Serialize(object item, Type type)
        {
            return JsonConvert.SerializeObject(item).Replace("\n","").Replace("\r", "");
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
