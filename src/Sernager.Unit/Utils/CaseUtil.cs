using Sernager.Core.Configs;
using Sernager.Core.Utils;
using System.Text;
using System.Text.Json;
using YamlDotNet.Core;

namespace Sernager.Unit.Utils;

public static class CaseUtil
{
    public static readonly string DEFAULT_START_PATH = Path.Combine(Environment.CurrentDirectory, "Cases");
    public static readonly string TEMP_START_PATH = Path.Combine(DEFAULT_START_PATH, "Temps");

    public static string GetPath(string alias, string extension)
    {
        if (!extension.StartsWith('.'))
        {
            extension = $".{extension}";
        }

        string aliasPath = alias.Replace('.', Path.DirectorySeparatorChar);
        string path = Path.Combine(DEFAULT_START_PATH, $"{aliasPath}{extension}");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Case file not found: Cases{Path.DirectorySeparatorChar}{aliasPath}{extension}");
        }

        return path;
    }

    public static string CreateTempFile(string alias, object obj)
    {
        byte[] bytes;

        if (obj is string str)
        {
            bytes = Encoding.Default.GetBytes(str);
        }
        else if (obj is byte[] byteArray)
        {
            bytes = byteArray;
        }
        else if (obj is int num)
        {
            bytes = BitConverter.GetBytes(num);
        }
        else
        {
            throw new ArgumentException($"Invalid type: {obj.GetType().Name}");
        }

        string aliasPath = alias.Replace('.', Path.DirectorySeparatorChar);
        string randomString = Randomizer.GenerateRandomString(20);
        string directoryPath = Path.Combine(TEMP_START_PATH, aliasPath);
        string path = Path.Combine(directoryPath, randomString);

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllBytes(path, bytes);

        return path;
    }

    public static void DeleteTempFiles(string alias)
    {
        string aliasPath = alias.Replace('.', Path.DirectorySeparatorChar);
        string path = Path.Combine(TEMP_START_PATH, aliasPath);

        if (!Directory.Exists(path))
        {
            return;
        }

        Directory.Delete(path, true);
    }

    public static byte[] Read(string alias, string extension)
    {
        string path = GetPath(alias, extension);

        return File.ReadAllBytes(path);
    }

    public static string ReadString(string alias, string extension)
    {
        string path = GetPath(alias, extension);

        Encoding encoding;

        using (StreamReader reader = new StreamReader(path))
        {
            encoding = reader.CurrentEncoding;
        }

        return encoding.GetString(File.ReadAllBytes(path));
    }

    public static T ReadYaml<T>(string alias)
        where T : class
    {
        string aliasPath = alias.Replace('.', Path.DirectorySeparatorChar);
        string yaml = ReadString(aliasPath, "yaml");

        T? result = YamlWrapper.Deserialize<T>(yaml);

        if (result == null)
        {
            throw new YamlException($"Failed to deserialize YAML to {typeof(T).Name}: Cases{Path.DirectorySeparatorChar}{aliasPath}.yaml");
        }

        return result;
    }

    public static T ReadJson<T>(string alias)
        where T : class
    {
        string aliasPath = alias.Replace('.', Path.DirectorySeparatorChar);
        string json = ReadString(aliasPath, "json");

        T? result = JsonWrapper.Deserialize<T>(json);

        if (result == null)
        {
            throw new JsonException($"Failed to deserialize JSON to {typeof(T).Name}: Cases{Path.DirectorySeparatorChar}{aliasPath}.json");
        }

        return result;
    }

    internal static Configuration ReadSernagerConfig(string alias)
    {
        string aliasPath = alias.Replace('.', Path.DirectorySeparatorChar);
        ConfigurationMetadata? metadata;

        using (ByteReader reader = new ByteReader(GetPath(aliasPath, "srn")))
        {
            metadata = PrivateUtil.GetMethodResult<ConfigurationMetadata>(typeof(ConfigurationMetadata), "fromSernagerBytes", reader);

            if (metadata == null)
            {
                throw new InvalidOperationException($"Failed to deserialize Sernager configuration: Cases{Path.DirectorySeparatorChar}{aliasPath}.srn");
            }
        }

        return metadata.Config;
    }
}
