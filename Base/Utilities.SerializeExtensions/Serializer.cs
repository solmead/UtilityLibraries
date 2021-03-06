﻿using Microsoft.Extensions.Logging;
using System;
using System.Text;
using Utilities.SerializeExtensions.Serializers;

namespace Utilities.SerializeExtensions
{
    public class Serializer : ISerializer
    {
        private readonly ILogger _logger;
        private readonly ISerializer serializer;
        [Obsolete("Just pass in ILogger in constructor", true)]
        public Action<string> LogMessage { get; set; }


        public Encoding BaseEncoding
        {
            get => serializer.BaseEncoding;
            set => serializer.BaseEncoding = value;
        }
        //[Obsolete("Use new Serializer(iLogger) or ISerializer", false)]
        public Serializer()
        {
            //_logger = logger;
            serializer = new JsonSerializer(_logger);
        }
        public Serializer(ILogger logger)
        {
            _logger = logger;
            serializer = new JsonSerializer(_logger);
        }
        public Serializer(ISerializer baseSerializer, ILogger logger)
        {
            _logger = logger;
            serializer = baseSerializer;
        }

        public T Deserialize<T>(string data) where T : class
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }
            var it = serializer.Deserialize<T>(data);

            if (it == null)
            {
                ISerializer ser = new JsonSerializer(_logger);
                it = ser.Deserialize<T>(data);
            }
            if (it == null)
            {
                ISerializer ser = new XmlSerializer(_logger);
                it = ser.Deserialize<T>(data);
            }
            if (it == null)
            {
                ISerializer ser = new BinarySerializer(_logger);
                it = ser.Deserialize<T>(data);
            }


            return it;
        }

        public object Deserialize(string data, Type type)
        {
            var it = serializer.Deserialize(data, type);

            if (it == null)
            {
                ISerializer ser = new JsonSerializer(_logger);
                it = ser.Deserialize(data, type);
            }
            if (it == null)
            {
                ISerializer ser = new XmlSerializer(_logger);
                it = ser.Deserialize(data, type);
            }
            if (it == null)
            {
                ISerializer ser = new BinarySerializer(_logger);
                it = ser.Deserialize(data, type);
            }


            return it;
        }

        public T Deserialize<T>(byte[] data) where T : class
        {
            var it =  serializer.Deserialize<T>(data);

            if (it == null)
            {
                ISerializer ser = new JsonSerializer(_logger);
                it = ser.Deserialize<T>(data);
            }
            if (it == null)
            {
                ISerializer ser = new XmlSerializer(_logger);
                it = ser.Deserialize<T>(data);
            }
            if (it == null)
            {
                ISerializer ser = new BinarySerializer(_logger);
                it = ser.Deserialize<T>(data);
            }


            return it;
        }

        public object Deserialize(byte[] data, Type type)
        {
            var it =  serializer.Deserialize(data, type);

            if (it == null)
            {
                ISerializer ser = new JsonSerializer(_logger);
                it = ser.Deserialize(data, type);
            }
            if (it == null)
            {
                ISerializer ser = new XmlSerializer(_logger);
                it = ser.Deserialize(data, type);
            }
            if (it == null)
            {
                ISerializer ser = new BinarySerializer(_logger);
                it = ser.Deserialize(data, type);
            }


            return it;
        }

        public string Serialize<T>(T item) where T : class
        {
            return serializer.Serialize(item);
        }

        public string Serialize(object item, Type type)
        {
            return serializer.Serialize(item, type);
        }

        public byte[] SerializeToArray<T>(T item) where T : class
        {
            return serializer.SerializeToArray(item);
        }

        public byte[] SerializeToArray(object item, Type type)
        {
            return serializer.SerializeToArray(item, type);
        }


        //public static string Serialize<T>(T item, bool tryXml = true) 
        //{
        //    var xml = "";
        //    if (tryXml)
        //    {
        //        try
        //        {
        //            xml = XmlSerializer.Serialize(item);
        //        }
        //        catch (Exception)
        //        {
        //            xml = "B" + BinarySerializer.Serialize(item);
        //        }
        //    }
        //    else
        //    {
        //            xml = "B" + BinarySerializer.Serialize(item);
        //    }
        //    return xml;

        //}

        //public static T Deserialize<T>(string xmlString)
        //{
        //    object o = null;
        //    if (string.IsNullOrWhiteSpace(xmlString))
        //    {
        //        return default(T);
        //    }
        //    if (xmlString.First() != 'B')
        //    {
        //        o = XmlSerializer.Deserialize<T>(xmlString);
        //    }
        //    else
        //    {
        //        o = BinarySerializer.Deserialize<T>(xmlString.Substring(1));
        //    }
        //    return (T)o;
        //}
    }
}
