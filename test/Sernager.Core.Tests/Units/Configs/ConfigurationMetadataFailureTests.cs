using Sernager.Core.Configs;
using Sernager.Core.Options;
using Sernager.Core.Utils;
using Sernager.Unit.Extensions;
using System.ComponentModel;
using System.Text;

namespace Sernager.Core.Tests.Units.Configs;

public class ConfigurationMetadataFailureTests : FailureFixture
{
    [DatapointSource]
    private static readonly EConfigurationType[] CONFIGURATION_TYPES = Enum.GetValues<EConfigurationType>();
    [DatapointSource]
    private static readonly EUserFriendlyConfigurationType[] USER_FRIENDLY_CONFIGURATION_TYPES = Enum.GetValues<EUserFriendlyConfigurationType>();

    [Test]
    public void Parse_ShouldThrow_WhenConfigurationTypeDoesNotExist()
    {
        string json = "{\"Hello\": \"World!\"}";
        byte[] bytes = Encoding.Default.GetBytes(json);

        using (ByteReader reader = new ByteReader(bytes))
        {
            TestNoneLevel(() => ConfigurationMetadata.Parse(reader, (EConfigurationType)9999), Is.TypeOf<ConfigurationMetadata>());
            TestExceptionLevel<InvalidEnumArgumentException>(() => ConfigurationMetadata.Parse(reader, (EConfigurationType)9999));
        }
    }

    [Theory]
    public void Parse_ShouldThrow_WhenInvalidBytesPassed(EConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        byte[] bytes;
        string configString = "InvalidConfiguration";

        if (type != EConfigurationType.Sernager)
        {
            bytes = Encoding.UTF8.GetBytes(configString);

            using (ByteReader reader = new ByteReader(bytes))
            {
                TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
                reader.ChangePosition(0);
                TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
            }

            return;
        }

        string key = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE);
        string iv = Randomizer.GenerateRandomString(Encryptor.IV_SIZE);

        string salt = "Salt";
        byte[] encrypted = Encryptor.Encrypt($"{salt}{configString}{salt}", key, iv);

        using (ByteWriter writer = new ByteWriter())
        {
            writer.WriteInt32(Encryptor.KEY_SIZE);
            bytes = writer.GetBytes();

            using (ByteReader reader = new ByteReader(bytes))
            {
                TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
                reader.ChangePosition(0);
                TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
            }

            writer.WriteInt32(Encryptor.IV_SIZE);
            bytes = writer.GetBytes();

            using (ByteReader reader = new ByteReader(bytes))
            {
                TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
                reader.ChangePosition(0);
                TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
            }

            writer.WriteInt32(salt.Length);
            bytes = writer.GetBytes();

            using (ByteReader reader = new ByteReader(bytes))
            {
                TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
                reader.ChangePosition(0);
                TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
            }

            writer.WriteInt32(salt.Length);
            bytes = writer.GetBytes();

            using (ByteReader reader = new ByteReader(bytes))
            {
                TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
                reader.ChangePosition(0);
                TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
            }

            writer.WriteString(Encoding.UTF8, key);
            bytes = writer.GetBytes();

            using (ByteReader reader = new ByteReader(bytes))
            {
                TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
                reader.ChangePosition(0);
                TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
            }

            writer.WriteString(Encoding.UTF8, iv);
            bytes = writer.GetBytes();

            using (ByteReader reader = new ByteReader(bytes))
            {
                TestNoneLevel(() => ConfigurationMetadata.Parse(reader, type), Is.TypeOf<ConfigurationMetadata>());
                reader.ChangePosition(0);
                TestExceptionLevel<SernagerException>(() => ConfigurationMetadata.Parse(reader, type));
            }

            writer.WriteBytes(encrypted);
            bytes = writer.GetBytes();

            using (ByteReader reader = new ByteReader(bytes))
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

        byte[] bytes = CaseUtil.Read("Configs.Defaults.Sernager", Configurator.GetExtension(type));

        using (ByteReader reader = new ByteReader(bytes))
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

        byte[] ufBytes = CaseUtil.Read("Configs.UserFriendlys.Sernager", Configurator.GetExtension(ufType));

        using (ByteReader reader = new ByteReader(ufBytes))
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

        byte[] bytes = CaseUtil.Read("Configs.Defaults.Sernager", Configurator.GetExtension(type));

        using (ByteReader reader = new ByteReader(bytes))
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

        byte[] ufBytes = CaseUtil.Read("Configs.UserFriendlys.Sernager", Configurator.GetExtension(ufType));

        using (ByteReader reader = new ByteReader(ufBytes))
        {
            using (ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type))
            {
                TestNoneLevel(() => metadata.ToBytes((EUserFriendlyConfigurationType)9999), Is.Empty);
                TestExceptionLevel<InvalidEnumArgumentException>(() => metadata.ToBytes((EUserFriendlyConfigurationType)9999));
            }
        }
    }
}
