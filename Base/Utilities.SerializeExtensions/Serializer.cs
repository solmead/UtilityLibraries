using System;
using System.Text;
using Utilities.SerializeExtensions.Serializers;

namespace Utilities.SerializeExtensions
{
    public class Serializer : ISerializer
    {
        private readonly ISerializer serializer;

        public Action<string> LogMessage {
            get => serializer.LogMessage;
            set => serializer.LogMessage = value;
        }
        public Encoding BaseEncoding
        {
            get => serializer.BaseEncoding;
            set => serializer.BaseEncoding = value;
        }

        public Serializer(ISerializer baseSerializer)
        {
            serializer = baseSerializer;
        }

        public T Deserialize<T>(string data)
        {
            return serializer.Deserialize<T>(data);
        }

        public object Deserialize(string data, Type type)
        {
            return serializer.Deserialize(data, type);
        }

        public T Deserialize<T>(byte[] data)
        {
            return serializer.Deserialize<T>(data);
        }

        public object Deserialize(byte[] data, Type type)
        {
            return serializer.Deserialize(data, type);
        }

        public string Serialize<T>(T item)
        {
            return serializer.Serialize(item);
        }

        public string Serialize(object item, Type type)
        {
            return serializer.Serialize(item, type);
        }

        public byte[] SerializeToArray<T>(T item)
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
