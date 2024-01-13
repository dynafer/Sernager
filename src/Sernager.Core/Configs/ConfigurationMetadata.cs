using Sernager.Core.Extensions;
using Sernager.Core.Helpers;
using Sernager.Core.Managers;
using Sernager.Core.Options;
using Sernager.Core.Utils;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Sernager.Core.Configs;

internal sealed class ConfigurationMetadata : IDisposable
{
    internal const int MIN_SIZE = 32;
    internal const int MAX_SIZE = 64;
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
                ExceptionManager.ThrowFail<InvalidEnumArgumentException>("type", (int)type, typeof(EConfigurationType));
                return new ConfigurationMetadata(new Configuration());
        }
    }

    internal byte[] ToBytes(EConfigurationType type)
    {
        if (Config == null)
        {
            ExceptionManager.ThrowFail<ObjectDisposedException>(nameof(ConfigurationMetadata));
            return Array.Empty<byte>();
        }

        switch (type)
        {
            case EConfigurationType.Yaml:
                return toYamlBytes(false);
            case EConfigurationType.Json:
                return toJsonBytes(false);
            case EConfigurationType.Sernager:
                return toSernagerBytes();
            default:
                ExceptionManager.ThrowFail<InvalidEnumArgumentException>("type", (int)type, typeof(EConfigurationType));
                return Array.Empty<byte>();
        }
    }

    internal byte[] ToBytes(EUserFriendlyConfigurationType type)
    {
        if (Config == null)
        {
            ExceptionManager.ThrowFail<ObjectDisposedException>(nameof(ConfigurationMetadata));
            return Array.Empty<byte>();
        }

        switch (type)
        {
            case EUserFriendlyConfigurationType.Yaml:
                return toYamlBytes(true);
            case EUserFriendlyConfigurationType.Json:
                return toJsonBytes(true);
            default:
                ExceptionManager.ThrowFail<InvalidEnumArgumentException>("type", (int)type, typeof(EUserFriendlyConfigurationType));
                return Array.Empty<byte>();
        }
    }

    private static ConfigurationMetadata fromYamlBytes(byte[] bytes)
    {
        string yaml = EncodingHelper.GetString(bytes);
        Configuration? config = YamlWrapper.Deserialize<Configuration>(yaml);

        if (config != null)
        {
            return new ConfigurationMetadata(config);
        }

        UserFriendlyConfiguration? userFriendlyConfig = YamlWrapper.Deserialize<UserFriendlyConfiguration>(yaml);

        if (userFriendlyConfig != null)
        {
            return new ConfigurationMetadata(userFriendlyConfig.ToConfiguration());
        }

        return failToDeserialize();
    }

    private static ConfigurationMetadata fromJsonBytes(byte[] bytes)
    {
        string json = EncodingHelper.GetString(bytes);
        Configuration? config = JsonWrapper.Deserialize<Configuration>(json);

        if (config != null)
        {
            return new ConfigurationMetadata(config);
        }

        UserFriendlyConfiguration? userFriendlyConfig = JsonWrapper.Deserialize<UserFriendlyConfiguration>(json);

        if (userFriendlyConfig != null)
        {
            return new ConfigurationMetadata(userFriendlyConfig.ToConfiguration());
        }

        return failToDeserialize();
    }

    private static ConfigurationMetadata fromSernagerBytes(byte[] bytes)
    {
        using (ByteReader reader = new ByteReader(bytes))
        {
            int keyLength;
            int ivLength;
            int beginSaltLength;
            int endSaltLength;

            if (!reader.TryReadInt32(out keyLength)
                || !reader.TryReadInt32(out ivLength)
                || !reader.TryReadInt32(out beginSaltLength)
                || !reader.TryReadInt32(out endSaltLength))
            {
                return failToDeserialize();
            }

            string? key;
            string? iv;

            if (!reader.TryReadString(Encoding.UTF8, keyLength, out key)
                || !reader.TryReadString(Encoding.UTF8, ivLength, out iv))
            {
                return failToDeserialize();
            }

            byte[]? encryptedBytes;

            if (!reader.TryReadBytes(reader.Length - reader.Position, out encryptedBytes))
            {
                return failToDeserialize();
            }

            string saltedData = Encryptor.Decrypt(encryptedBytes, key, iv);
            saltedData = saltedData.Substring(beginSaltLength, saltedData.Length - beginSaltLength - endSaltLength);

            Configuration? config = JsonWrapper.Deserialize<Configuration>(saltedData);

            if (config == null)
            {
                return failToDeserialize();
            }

            return new ConfigurationMetadata(config);
        }
    }

    [StackTraceHidden]
    private static ConfigurationMetadata failToDeserialize()
    {
        ExceptionManager.ThrowFail<SernagerException>("Failed to deserialize configuration file.");
        return new ConfigurationMetadata(new Configuration());
    }

    private byte[] toJsonBytes(bool bUserFriendly)
    {
        string json = JsonWrapper.Serialize(
            bUserFriendly
                ? Config.ToUserFriendlyConfiguration()
                : Config,
            bUserFriendly
        );

        return Encoding.Default.GetBytes(json);
    }

    private byte[] toYamlBytes(bool bUserFriendly)
    {
        string yaml = YamlWrapper.Serialize(
            bUserFriendly
                ? Config.ToUserFriendlyConfiguration()
                : Config
        );

        return Encoding.Default.GetBytes(yaml);
    }

    private byte[] toSernagerBytes()
    {
        string key = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE);
        string iv = Randomizer.GenerateRandomString(Encryptor.IV_SIZE);
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
                .WriteString(Encoding.UTF8, key)
                .WriteString(Encoding.UTF8, iv)
                .WriteBytes(encrypted);

            return writer.GetBytes();
        }
    }
}
