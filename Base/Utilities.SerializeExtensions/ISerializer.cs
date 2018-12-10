using System;

namespace Utilities.SerializeExtensions.Serializers
{
    public interface ISerializer
    {
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
