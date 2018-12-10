using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Utilities.SerializeExtensions.Serializers
{
    public class XmlSerializer : ISerializer
    {
        public T Deserialize<T>(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return default(T);
            
            using (var memStream = new MemoryStream(Encoding.Unicode.GetBytes(data)))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(memStream);
            }
        }

        public object Deserialize(string data, Type type)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;
            
            using (var memStream = new MemoryStream(Encoding.Unicode.GetBytes(data)))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(type);
                return serializer.Deserialize(memStream);
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            var s = System.Text.Encoding.UTF8.GetString(data);
            return Deserialize<T>(s);
        }

        public object Deserialize(byte[] data, Type type)
        {
            var s = System.Text.Encoding.UTF8.GetString(data);
            return Deserialize(s, type);
        }

        public string Serialize<T>(T item)
        {
            var memStream = new MemoryStream();
            using (XmlTextWriter textWriter = new XmlTextWriter(memStream, Encoding.Unicode))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                serializer.Serialize(textWriter, item);

                memStream = textWriter.BaseStream as MemoryStream;
            }
            if (memStream != null)
                return Encoding.Unicode.GetString(memStream.ToArray());
            else
                return null;
        }

        public string Serialize(object item, Type type)
        {
            var memStream = new MemoryStream();
            using (XmlTextWriter textWriter = new XmlTextWriter(memStream, Encoding.Unicode))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(type);
                serializer.Serialize(textWriter, item);

                memStream = textWriter.BaseStream as MemoryStream;
            }
            if (memStream != null)
                return Encoding.Unicode.GetString(memStream.ToArray());
            else
                return null;
        }

        public byte[] SerializeToArray<T>(T item)
        {
            var memStream = new MemoryStream();
            using (XmlTextWriter textWriter = new XmlTextWriter(memStream, Encoding.Unicode))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                serializer.Serialize(textWriter, item);

                memStream = textWriter.BaseStream as MemoryStream;
            }
            if (memStream != null)
                return memStream.ToArray();
            else
                return null;
        }

        public byte[] SerializeToArray(object item, Type type)
        {
            var memStream = new MemoryStream();
            using (XmlTextWriter textWriter = new XmlTextWriter(memStream, Encoding.Unicode))
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
