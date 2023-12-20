using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Sernager.Core.Utils;

internal static class YamlWrapper
{
    internal static string Serialize(object obj)
    {
        if (obj == null)
        {
            return string.Empty;
        }

        ISerializer serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        return serializer.Serialize(obj);
    }

    internal static T? Deserialize<T>(string yaml)
    {
        try
        {
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<T>(yaml);
        }
        catch
        {
            return default;
        }
    }
}