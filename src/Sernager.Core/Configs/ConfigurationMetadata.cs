using Sernager.Core.Managers;
using Sernager.Core.Options;
using Sernager.Core.Utils;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using YamlDotNet.Core;

namespace Sernager.Core.Configs;

internal sealed class ConfigurationMetadata : IDisposable
{
    internal static readonly int KEY_SIZE = 32;
    internal static readonly int IV_SIZE = 16;
    internal static readonly int MIN_SIZE = 32;
    internal static readonly int MAX_SIZE = 64;
    internal Configuration Config { get; private set; } = null!;

    internal ConfigurationMetadata(Configuration config)
    {
        Config = config;
    }

    public void Dispose()
    {
        Config = null!;
    }

    internal static ConfigurationMetadata Parse(ByteReader reader, EConfigurationType type)
    {
        switch (type)
        {
            case EConfigurationType.Yaml:
                return fromYamlBytes(reader.ReadBytes(reader.Length - reader.Position));
            case EConfigurationType.Json:
                return fromJsonBytes(reader.ReadBytes(reader.Length - reader.Position));
            case EConfigurationType.Sernager:
                return fromSernagerBytes(reader.ReadBytes(reader.Length - reader.Position));
            default:
                ErrorManager.ThrowFail<InvalidEnumArgumentException>("type", (int)type, typeof(EConfigurationType));
                return new ConfigurationMetadata(new Configuration());
        }
    }

    internal byte[] ToBytes(EConfigurationType type)
    {
        switch (type)
        {
            case EConfigurationType.Yaml:
                return toYamlBytes();
            case EConfigurationType.Json:
                return toJsonBytes();
            case EConfigurationType.Sernager:
                return toSernagerBytes();
            default:
                ErrorManager.ThrowFail<InvalidEnumArgumentException>("type", (int)type, typeof(EConfigurationType));
                return Array.Empty<byte>();
        }
    }

    private static ConfigurationMetadata fromYamlBytes(byte[] bytes)
    {
        string yaml = Encoding.UTF8.GetString(bytes);
        Configuration? config = YamlWrapper.Deserialize<Configuration>(yaml);

        if (config == null)
        {
            ErrorManager.ThrowFail<YamlException>("Failed to deserialize configuration.");
            return new ConfigurationMetadata(new Configuration());
        }

        return new ConfigurationMetadata(config);
    }

    private static ConfigurationMetadata fromJsonBytes(byte[] bytes)
    {
        string json = Encoding.UTF8.GetString(bytes);
        Configuration? config = JsonWrapper.Deserialize<Configuration>(json);

        if (config == null)
        {
            ErrorManager.ThrowFail<JsonException>("Failed to deserialize configuration.");
            return new ConfigurationMetadata(new Configuration());
        }

        return new ConfigurationMetadata(config);
    }

    private static ConfigurationMetadata fromSernagerBytes(byte[] bytes)
    {
        using (ByteReader reader = new ByteReader(bytes))
        {
            int keyLength = reader.ReadInt32();
            int ivLength = reader.ReadInt32();
            int beginSaltLength = reader.ReadInt32();
            int endSaltLength = reader.ReadInt32();

            string key = reader.ReadString(keyLength);
            string iv = reader.ReadString(ivLength);
            byte[] encryptedBytes = reader.ReadBytes(reader.Length - reader.Position);

            string saltedData = Encryptor.Decrypt(encryptedBytes, key, iv);
            saltedData = saltedData.Substring(beginSaltLength, saltedData.Length - beginSaltLength - endSaltLength);
            Configuration? config = JsonWrapper.Deserialize<Configuration>(saltedData);

            if (config == null)
            {
                ErrorManager.ThrowFail<SernagerException>("Failed to deserialize configuration.");
                return new ConfigurationMetadata(new Configuration());
            }

            return new ConfigurationMetadata(config);
        }
    }

    private byte[] toJsonBytes()
    {
        string json = JsonWrapper.Serialize(Config, true);

        return Encoding.UTF8.GetBytes(json);
    }

    private byte[] toYamlBytes()
    {
        string yaml = YamlWrapper.Serialize(Config);

        return Encoding.UTF8.GetBytes(yaml);
    }

    private byte[] toSernagerBytes()
    {
        string key = Randomizer.GenerateRandomString(KEY_SIZE);
        string iv = Randomizer.GenerateRandomString(IV_SIZE);
        string[] salts =
        {
            Randomizer.GenerateRandomString(MIN_SIZE, MAX_SIZE),
            Randomizer.GenerateRandomString(MIN_SIZE, MAX_SIZE)
        };

        string saltedData = $"{salts[0]}{JsonWrapper.Serialize(Config)}{salts[1]}";
        byte[] encrypted = Encryptor.Encrypt(saltedData, key, iv);

        using (ByteWriter writer = new ByteWriter())
        {
            writer.WriteInt32(key.Length)
                .WriteInt32(iv.Length)
                .WriteInt32(salts[0].Length)
                .WriteInt32(salts[1].Length)
                .WriteString(key)
                .WriteString(iv)
                .WriteBytes(encrypted);

            return writer.GetBytes();
        }
    }
}
