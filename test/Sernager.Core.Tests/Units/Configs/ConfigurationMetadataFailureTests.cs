using Sernager.Core.Configs;
using Sernager.Core.Options;
using Sernager.Core.Utils;
using Sernager.Unit.Extensions;
using System.ComponentModel;
using System.Text;

namespace Sernager.Core.Tests.Units.Configs;

public class ConfigurationMetadataFailureTests : FailureFixture
{
    private static readonly string TEMP_FILE_ALIAS = "ConfigurationMetadata";
    [DatapointSource]
    private static readonly EConfigurationType[] CONFIGURATION_TYPES = Enum.GetValues<EConfigurationType>();
    [DatapointSource]
    private static readonly EUserFriendlyConfigurationType[] USER_FRIENDLY_CONFIGURATION_TYPES = Enum.GetValues<EUserFriendlyConfigurationType>();
    [DatapointSource]
    private static readonly Encoding[] ENCODING_LIST =
    [
        Encoding.UTF8,
        Encoding.Unicode,
        Encoding.BigEndianUnicode,
        Encoding.UTF32,
        Encoding.ASCII,
        Encoding.Default,
    ];

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        CaseUtil.DeleteTempFiles(TEMP_FILE_ALIAS);
    }

    [Test]
    public void Parse_ShouldThrow_WhenConfigurationTypeDoesNotExist()
    {
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, "{\"Hello\": \"World!\"}");

        using (ByteReader reader = new ByteReader(path))
        {
            TestNoneLevel(() => ConfigurationMetadata.Parse(reader, (EConfigurationType)9999), Is.TypeOf<ConfigurationMetadata>());
            TestExceptionLevel<InvalidEnumArgumentException>(() => ConfigurationMetadata.Parse(reader, (EConfigurationType)9999));
        }
    }

    [Theory]
    public void Parse_ShouldThrow_WhenInvalidBytesPassed(EConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        if (type == EConfigurationType.Sernager)
        {
            Assert.Pass();
            return;
        }

        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, "InvalidConfiguration");

        using (ByteReader reader = new ByteReader(path))
        {
            TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
            reader.ChangePosition(0);
            TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
        }

        path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, "");

        using (ByteReader reader = new ByteReader(path))
        {
            TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
            reader.ChangePosition(0);
            TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
        }
    }

    [Theory]
    public void Parse_ShouldThrow_WhenInvalidBytesPassed_SernagerType(Encoding encoding)
    {
        Assume.That(encoding, Is.AnyOf(ENCODING_LIST));

        EConfigurationType type = EConfigurationType.Sernager;
        string path;
        string configString = "InvalidConfiguration";
        string key = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE);
        string iv = Randomizer.GenerateRandomString(Encryptor.IV_SIZE);

        string salt = "Salt";
        string encryptingString = $"{salt}{configString}{salt}";
        encryptingString = Encoding.UTF8.GetString(encoding.GetBytes(encryptingString));
        byte[] encrypted = Encryptor.Encrypt(encryptingString, key, iv);

        using (ByteWriter writer = new ByteWriter())
        {
            writer.WriteInt32(Encoding.UTF8.GetByteCount(key));
            path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, writer.GetBytes());

            using (ByteReader reader = new ByteReader(path))
            {
                TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
                reader.ChangePosition(0);
                TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
            }

            writer.WriteInt32(Encoding.UTF8.GetByteCount(iv));
            path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, writer.GetBytes());

            using (ByteReader reader = new ByteReader(path))
            {
                TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
                reader.ChangePosition(0);
                TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
            }

            writer.WriteInt32(salt.Length);
            path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, writer.GetBytes());

            using (ByteReader reader = new ByteReader(path))
            {
                TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
                reader.ChangePosition(0);
                TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
            }

            writer.WriteInt32(salt.Length);
            path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, writer.GetBytes());

            using (ByteReader reader = new ByteReader(path))
            {
                TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
                reader.ChangePosition(0);
                TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
            }

            writer.WriteString(Encoding.UTF8, key);
            path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, writer.GetBytes());

            using (ByteReader reader = new ByteReader(path))
            {
                TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
                reader.ChangePosition(0);
                TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
            }

            writer.WriteString(Encoding.UTF8, iv);
            path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, writer.GetBytes());

            using (ByteReader reader = new ByteReader(path))
            {
                TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
                reader.ChangePosition(0);
                TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
            }

            writer.WriteBytes(encrypted);
            path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, writer.GetBytes());

            using (ByteReader reader = new ByteReader(path))
            {
                TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
                reader.ChangePosition(0);
                TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
            }
        }
    }

    [Theory]
    public void ToBytes_ShouldThrow_WhenDisposed(EConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        string path = CaseUtil.GetPath("Configs.Defaults.Sernager", Configurator.GetExtension(type));

        using (ByteReader reader = new ByteReader(path))
        {
            using (ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type))
            {
                metadata.Dispose();

                TestNoneLevel(() => metadata.ToBytes(type), Is.Empty);
                TestExceptionLevel<ObjectDisposedException>(() => metadata.ToBytes(type));
            }
        }
    }

    [Theory]
    public void ToBytes_ShouldThrow_WhenDisposed(EUserFriendlyConfigurationType ufType)
    {
        Assume.That(ufType, Is.AnyOf(USER_FRIENDLY_CONFIGURATION_TYPES));

        EConfigurationType type = ufType switch
        {
            EUserFriendlyConfigurationType.Yaml => EConfigurationType.Yaml,
            EUserFriendlyConfigurationType.Json => EConfigurationType.Json,
            _ => throw new InvalidEnumArgumentException(nameof(ufType), (int)ufType, typeof(EUserFriendlyConfigurationType))
        };

        string path = CaseUtil.GetPath("Configs.UserFriendlys.Sernager", Configurator.GetExtension(ufType));

        using (ByteReader reader = new ByteReader(path))
        {
            using (ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type))
            {
                metadata.Dispose();

                TestNoneLevel(() => metadata.ToBytes(ufType), Is.Empty);
                TestExceptionLevel<ObjectDisposedException>(() => metadata.ToBytes(ufType));
            }
        }
    }

    [Theory]
    public void ToBytes_ShouldThrow_WhenTypeDoesnExist(EConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        string path = CaseUtil.GetPath("Configs.Defaults.Sernager", Configurator.GetExtension(type));

        using (ByteReader reader = new ByteReader(path))
        {
            using (ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type))
            {
                TestNoneLevel(() => metadata.ToBytes((EConfigurationType)9999), Is.Empty);
                TestExceptionLevel<InvalidEnumArgumentException>(() => metadata.ToBytes((EConfigurationType)9999));
            }
        }
    }

    [Theory]
    public void ToBytes_ShouldThrow_WhenTypeDoesnExist(EUserFriendlyConfigurationType ufType)
    {
        Assume.That(ufType, Is.AnyOf(USER_FRIENDLY_CONFIGURATION_TYPES));

        EConfigurationType type = ufType switch
        {
            EUserFriendlyConfigurationType.Yaml => EConfigurationType.Yaml,
            EUserFriendlyConfigurationType.Json => EConfigurationType.Json,
            _ => throw new InvalidEnumArgumentException(nameof(ufType), (int)ufType, typeof(EUserFriendlyConfigurationType))
        };

        string path = CaseUtil.GetPath("Configs.UserFriendlys.Sernager", Configurator.GetExtension(ufType));

        using (ByteReader reader = new ByteReader(path))
        {
            using (ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type))
            {
                TestNoneLevel(() => metadata.ToBytes((EUserFriendlyConfigurationType)9999), Is.Empty);
                TestExceptionLevel<InvalidEnumArgumentException>(() => metadata.ToBytes((EUserFriendlyConfigurationType)9999));
            }
        }
    }
}
