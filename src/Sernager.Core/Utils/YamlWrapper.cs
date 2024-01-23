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
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithQuotingNecessaryStrings(true)
            .Build();

        return serializer.Serialize(obj);
    }

    internal static T? Deserialize<T>(string yaml)
        where T : class
    {
        try
        {
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<T>(yaml);
        }
        catch
        {
            return null;
        }
    }
}
