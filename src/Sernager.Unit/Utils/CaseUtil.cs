using Sernager.Core.Configs;
using Sernager.Core.Utils;
using System.Reflection;
using System.Text.Json;
using YamlDotNet.Core;

namespace Sernager.Unit.Utils;

public static class CaseUtil
{
    public static readonly string DEFAULT_START_PATH = Path.Combine(Environment.CurrentDirectory, "Cases");

    public static byte[] Read(string alias, string extension)
    {
        if (!extension.StartsWith('.'))
        {
            extension = $".{extension}";
        }

        string aliasPath = alias.Replace('.', Path.DirectorySeparatorChar);
        string path = Path.Combine(DEFAULT_START_PATH, $"{aliasPath}{extension}");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Case file not found: Cases/{aliasPath}.{extension}");
        }

        return File.ReadAllBytes(path);
    }

    public static string ReadString(string alias, string extension)
    {
        if (!extension.StartsWith('.'))
        {
            extension = $".{extension}";
        }

        string aliasPath = alias.Replace('.', Path.DirectorySeparatorChar);
        string path = Path.Combine(DEFAULT_START_PATH, $"{aliasPath}{extension}");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Case file not found: Cases/{aliasPath}.{extension}");
        }

        return File.ReadAllText(path);
    }

    public static T ReadJson<T>(string alias)
    {
        string aliasPath = alias.Replace('.', Path.DirectorySeparatorChar);
        string json = ReadString(aliasPath, "json");

        T? result = JsonSerializer.Deserialize<T>(json);

        if (result == null)
        {
            throw new JsonException($"Failed to deserialize JSON to {typeof(T).Name}: Cases/{aliasPath}.json");
        }

        return result;
    }

    public static T ReadYaml<T>(string alias)
    {
        string aliasPath = alias.Replace('.', Path.DirectorySeparatorChar);
        string yaml = ReadString(aliasPath, "yaml");

        T? result = YamlWrapper.Deserialize<T>(yaml);

        if (result == null)
        {
            throw new YamlException($"Failed to deserialize YAML to {typeof(T).Name}: Cases/{aliasPath}.yaml");
        }

        return result;
    }

    internal static Configuration ReadSernagerConfig(string alias)
    {
        string aliasPath = alias.Replace('.', Path.DirectorySeparatorChar);
        byte[] bytes = Read(aliasPath, "srn");

        MethodInfo? method = typeof(ConfigurationMetadata).GetMethod("fromSernagerBytes", BindingFlags.NonPublic | BindingFlags.Static);
        if (method == null)
        {
            throw new MissingMethodException("fromSernagerBytes");
        }

        object? result = method.Invoke(null, [bytes]);

        if (result is not ConfigurationMetadata metadata)
        {
            throw new Exception("Failed to deserialize Sernager to ConfigurationMetadata.");
        }

        return metadata.Config;
    }
}
