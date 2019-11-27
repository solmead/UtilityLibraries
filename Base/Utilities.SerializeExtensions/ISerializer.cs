using System;
using System.Text;

namespace Utilities.SerializeExtensions.Serializers
{
    public interface ISerializer
    {
        Action<string> LogMessage { get; set; }
        Encoding BaseEncoding { get; set; }
        string Serialize<T>(T item);
        string Serialize(object item, Type type);
        byte[] SerializeToArray<T>(T item);
        byte[] SerializeToArray(object item, Type type);
        T Deserialize<T>(string data);
        object Deserialize(string data, Type type);
        T Deserialize<T>(byte[] data);
        object Deserialize(byte[] data, Type type);

    }
}
