using ServiceRunner.Runner.Utils;

namespace ServiceRunner.Runner.Configs;

/// <include file='docs/configs/configuration_metadata.xml' path='Class/Description'/>
internal sealed class ConfigurationMetadata : IDisposable
{
    internal static readonly int KEY_SIZE = 32;
    internal static readonly int IV_SIZE = 16;
    internal static readonly int MIN_SIZE = 32;
    internal static readonly int MAX_SIZE = 64;
    internal Configuration Config { get; private set; }

    /// <include file='docs/configs/configuration_metadata.xml' path='Class/Constructor[@Name="WithConfig"]'/>
    internal ConfigurationMetadata(Configuration config)
    {
        Config = config;
    }

    /// <include file='docs/configs/configuration_metadata.xml' path='Class/Constructor[@Name="WithByteReader"]'/>
    internal ConfigurationMetadata(ByteReader reader)
    {
        int keyLength = reader.ReadInt32();
        int ivLength = reader.ReadInt32();
        int beginSaltLength = reader.ReadInt32();
        int endSaltLength = reader.ReadInt32();

        reader.Skip(beginSaltLength);
        string key = reader.ReadString(keyLength);
        string iv = reader.ReadString(ivLength);
        byte[] encryptedBytes = reader.ReadBytes(reader.Length - reader.Position - endSaltLength);
        reader.Skip(endSaltLength);

        string json = Encryptor.Decrypt(encryptedBytes, key, iv);
        Config = JsonWrapper.Deserialize<Configuration>(json) ?? throw new Exception("Failed to deserialize configuration");
    }

    public void Dispose()
    {
        Config = null!;
    }

    /// <include file='docs/configs/configuration_metadata.xml' path='Class/InternalMethod[@Name="ToBytes"]'/>
    internal byte[] ToBytes()
    {
        string key = Randomizer.GenerateRandomString(KEY_SIZE);
        string iv = Randomizer.GenerateRandomString(IV_SIZE);
        string[] salts =
        {
            Randomizer.GenerateRandomString(MIN_SIZE, MAX_SIZE),
            Randomizer.GenerateRandomString(MIN_SIZE, MAX_SIZE)
        };

        string json = JsonWrapper.Serialize(Config);
        byte[] encrypted = Encryptor.Encrypt(json, key, iv);

        using (ByteWriter writer = new ByteWriter())
        {
            writer.WriteInt32(key.Length)
                .WriteInt32(iv.Length)
                .WriteInt32(salts[0].Length)
                .WriteInt32(salts[1].Length)
                .WriteString(salts[0])
                .WriteString(key)
                .WriteString(iv)
                .WriteBytes(encrypted)
                .WriteString(salts[1]);

            return writer.GetBytes();
        }
    }
}
