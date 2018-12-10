//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Xml.Serialization;

//namespace System.Collections.Extensions
//{
//    [Serializable]
//    [XmlRoot("List")]
//    public class SerializableList<TValue> : List<TValue>, IXmlSerializable
//    {
//        #region IXmlSerializable Members
//        public System.Xml.Schema.XmlSchema GetSchema()
//        {
//            return null;
//        }
//        public void ReadXml(System.Xml.XmlReader reader)
//        {
//            var valueSerializer = new System.Xml.Serialization.XmlSerializer(typeof(TValue));
//            bool wasEmpty = reader.IsEmptyElement;
//            reader.Read();
//            if (wasEmpty)
//                return;
//            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
//            {
//                reader.ReadStartElement("item");
//                TValue value = (TValue)valueSerializer.Deserialize(reader);
//                this.Add(value);
//                reader.ReadEndElement();
//                reader.MoveToContent();
//            }
//            reader.ReadEndElement();
//        }
//        public void WriteXml(System.Xml.XmlWriter writer)
//        {
//            var valueSerializer = new System.Xml.Serialization.XmlSerializer(typeof(TValue));
//            foreach (TValue value in this)
//            {
//                writer.WriteStartElement("item");
//                valueSerializer.Serialize(writer, value);
//                writer.WriteEndElement();
//            }
//        }
//        #endregion
//    }
//}
