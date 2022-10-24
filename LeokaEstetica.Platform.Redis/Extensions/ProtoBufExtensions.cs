using ProtoBuf;

namespace LeokaEstetica.Platform.Redis.Extensions;

/// <summary>
/// Класс расширений для ProtoBuf.
/// </summary>
public static class ProtoBufExtensions
{
    public static string Serialize<T>(T instance)
    {
        using var stream = new MemoryStream();
        Serializer.Serialize(stream, instance);

        return Convert.ToBase64String(stream.ToArray());
    }

    public static T Deserialize<T>(string source)
    {
        using var stream = new MemoryStream(Convert.FromBase64String(source));
        
        return Serializer.Deserialize<T>(stream);
    }

    public static byte[] SerializeFromBinary<T>(T instance)
    {
        using var stream = new MemoryStream();
        Serializer.Serialize(stream, instance);

        return stream.ToArray();
    }
        
    public static Stream SerializeFromBinaryStream<T>(T instance)
    {
        var stream = new MemoryStream();

        Serializer.Serialize(stream, instance);
        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }

    public static T DeserializeFromBinary<T>(byte[] source)
    {
        using var stream = new MemoryStream(source);
        
        return Serializer.Deserialize<T>(stream);
    }

    public static object DeserializeFromBinary(Type type, byte[] source)
    {
        using var stream = new MemoryStream(source);
        
        return Serializer.NonGeneric.Deserialize(type, stream);
    }
}