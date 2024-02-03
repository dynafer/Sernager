using Sernager.Core.Configs;
using Sernager.Core.Options;
using Sernager.Core.Utils;
using Sernager.Unit.Extensions;
using System.ComponentModel;
using System.Text;

namespace Sernager.Core.Tests.Units.Configs;

internal sealed class ConfigurationMetadataSuccessTests
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
    ];

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        CaseUtil.DeleteTempFiles(TEMP_FILE_ALIAS);
    }

    [Theory]
    public void Parse_ShouldReturnConfigurationMetadata(EConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        string path = CaseUtil.GetPath("Configs.Defaults.Sernager", Configurator.GetExtension(type));

        using (ByteReader reader = new ByteReader(path))
        {
            ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type);

            Assert.That(metadata.Config, Is.Not.Null);
            Assert.That(metadata.Config, Is.TypeOf<Configuration>());
        }
    }

    [Theory]
    public void Parse_ShouldReturnConfigurationMetadata_SernagerType(Encoding encoding)
    {
        Assume.That(encoding, Is.AnyOf(ENCODING_LIST));

        string path = CaseUtil.GetPath($"Configs.Defaults.Sernager.{encoding.GetEncodingName()}", Configurator.GetExtension(EConfigurationType.Sernager));

        using (ByteReader reader = new ByteReader(path))
        {
            ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, EConfigurationType.Sernager);

            Assert.That(metadata.Config, Is.Not.Null);
            Assert.That(metadata.Config, Is.TypeOf<Configuration>());
        }
    }

    [Theory]
    public void Parse_ShouldReturnConfigurationMetadata(EUserFriendlyConfigurationType ufType)
    {
        Assume.That(ufType, Is.AnyOf(USER_FRIENDLY_CONFIGURATION_TYPES));

        string path = CaseUtil.GetPath("Configs.UserFriendlys.Sernager", Configurator.GetExtension(ufType));

        EConfigurationType type = ufType switch
        {
            EUserFriendlyConfigurationType.Yaml => EConfigurationType.Yaml,
            EUserFriendlyConfigurationType.Json => EConfigurationType.Json,
            _ => throw new InvalidEnumArgumentException(nameof(ufType), (int)ufType, typeof(EUserFriendlyConfigurationType))
        };

        using (ByteReader reader = new ByteReader(path))
        {
            ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type);

            Assert.That(metadata.Config, Is.Not.Null);
            Assert.That(metadata.Config, Is.TypeOf<Configuration>());
        }
    }

    [Theory]
    public void ToBytes_ShouldReturnBytes(EConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        string path = CaseUtil.GetPath("Configs.Defaults.Sernager", Configurator.GetExtension(type));
        byte[] bytes = CaseUtil.Read("Configs.Defaults.Sernager", Configurator.GetExtension(type));

        using (ByteReader reader = new ByteReader(path))
        {
            ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type);

            byte[] result = metadata.ToBytes(type);
            string resultPath = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, result);

            if (type != EConfigurationType.Sernager)
            {
                Assert.That(result, Is.Not.Empty);
                Assert.That(result, Is.EqualTo(bytes));

                return;
            }

            string expectJson = JsonWrapper.Serialize(metadata.Config);

            using (ByteReader resultReader = new ByteReader(resultPath))
            {
                ConfigurationMetadata resultMetadata = ConfigurationMetadata.Parse(resultReader, type);

                string resultJson = JsonWrapper.Serialize(resultMetadata.Config);

                Assert.That(resultJson, Is.EqualTo(expectJson));
            }
        }
    }

    [Theory]
    public void ToBytes_ShouldReturnBytes(EUserFriendlyConfigurationType ufType)
    {
        Assume.That(ufType, Is.AnyOf(USER_FRIENDLY_CONFIGURATION_TYPES));

        string path = CaseUtil.GetPath("Configs.UserFriendlys.Sernager", Configurator.GetExtension(ufType));
        byte[] ufBytes = CaseUtil.Read("Configs.UserFriendlys.Sernager", Configurator.GetExtension(ufType));

        EConfigurationType type = ufType switch
        {
            EUserFriendlyConfigurationType.Yaml => EConfigurationType.Yaml,
            EUserFriendlyConfigurationType.Json => EConfigurationType.Json,
            _ => throw new InvalidEnumArgumentException(nameof(ufType), (int)ufType, typeof(EUserFriendlyConfigurationType))
        };

        using (ByteReader reader = new ByteReader(path))
        {
            ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type);

            byte[] result = metadata.ToBytes(ufType);

            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Is.EqualTo(ufBytes));
        }
    }
}
