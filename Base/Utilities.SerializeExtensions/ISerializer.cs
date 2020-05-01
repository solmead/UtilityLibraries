using System;
using System.Text;

namespace Utilities.SerializeExtensions.Serializers
{
    public interface ISerializer
    {
        Action<string> LogMessage { get; set; }
        Encoding BaseEncoding { get; set; }
        string Serialize<T>(T item) where T: class;
        string Serialize(object item, Type type);
        byte[] SerializeToArray<T>(T item) where T : class;
        byte[] SerializeToArray(object item, Type type);
        T Deserialize<T>(string data) where T : class;
        object Deserialize(string data, Type type);
        T Deserialize<T>(byte[] data) where T : class;
        object Deserialize(byte[] data, Type type);

    }
}
