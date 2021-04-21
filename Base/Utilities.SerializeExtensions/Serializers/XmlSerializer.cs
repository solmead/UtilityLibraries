using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace Utilities.SerializeExtensions.Serializers
{
    public class XmlSerializer : ISerializer
    {

        public Encoding BaseEncoding { get; set; } = Encoding.Unicode;



        private readonly ILogger _logger = null;
        public XmlSerializer()
        {
            
        }
        public XmlSerializer(ILogger logger)
        {
            _logger = logger;
        }

        public T Deserialize<T>(string data) where T : class
        {
            return (T) Deserialize(data, typeof(T));
        }
        public T Deserialize<T>(byte[] data) where T : class
        {
            return (T)Deserialize(data, typeof(T));
        }
        public object Deserialize(string data, Type type)
        {
            if (string.IsNullOrWhiteSpace(data))
            {

                _logger?.LogDebug("Deserialize: null data");
                return null;
            }

            object obj = null;
            //baseEncoding
            _logger?.LogDebug("Deserialize - BaseEncoding:" + BaseEncoding.EncodingName);
            var encoding = BaseEncoding;
            var s = encoding.GetBytes(data);
            obj = Deserialize(s, type);

            if (obj == null)
            {
                encoding = Encoding.Unicode;
                _logger?.LogDebug("Deserialize - Encoding:" + encoding.EncodingName);
                s = encoding.GetBytes(data);
                obj = Deserialize(s, type);
            }
            if (obj == null)
            {
                _logger?.LogDebug("Deserialize - Encoding:" + encoding.EncodingName);
                encoding = Encoding.UTF32;
                s = encoding.GetBytes(data);
                obj = Deserialize(s, type);
            }
            if (obj == null)
            {
                _logger?.LogDebug("Deserialize - Encoding:" + encoding.EncodingName);
                encoding = Encoding.ASCII;
                s = encoding.GetBytes(data);
                obj = Deserialize(s, type);
            }
            if (obj == null)
            {
                _logger?.LogDebug("Deserialize - Encoding:" + encoding.EncodingName);
                encoding = Encoding.UTF8;
                s = encoding.GetBytes(data);
                obj = Deserialize(s, type);
            }
            if (obj == null)
            {
                _logger?.LogDebug("Deserialize - Encoding:" + encoding.EncodingName);
                encoding = Encoding.UTF7;
                s = encoding.GetBytes(data);
                obj = Deserialize(s, type);
            }


            return obj;
        }

        public object Deserialize(byte[] data, Type type)
        {
            _logger?.LogDebug("Deserialize - data[]:" + (data?.Length ?? 0));


            if ((data?.Length ?? 0) == 0)
                return null;

            try
            {
                using (var memStream = new MemoryStream(data))
                {
                    _logger?.LogDebug("Deserialize - memStream:" + memStream.Length);
                    var serializer = new System.Xml.Serialization.XmlSerializer(type);
                    return serializer.Deserialize(memStream);
                }
            }
            catch (Exception ex)
            {

                _logger?.LogError(ex, "Deserialize - Exception:" + ex.ToString());
                return null;
            }
        }




        public string Serialize<T>(T item) where T : class
        {
            return Serialize(item, typeof(T));
        }

        public string Serialize(object item, Type type)
        {
            return BaseEncoding.GetString(SerializeToArray(item, type));
        }

        public byte[] SerializeToArray<T>(T item) where T : class
        {
            return SerializeToArray(item, typeof(T));
        }

        public byte[] SerializeToArray(object item, Type type)
        {
            var memStream = new MemoryStream();
            using (XmlTextWriter textWriter = new XmlTextWriter(memStream, BaseEncoding))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(type);
                serializer.Serialize(textWriter, item);

                memStream = textWriter.BaseStream as MemoryStream;
            }
            if (memStream != null)
                return memStream.ToArray();
            else
                return null;
        }


    }
}
