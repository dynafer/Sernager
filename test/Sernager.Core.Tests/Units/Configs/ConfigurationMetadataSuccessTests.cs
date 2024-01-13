using Sernager.Core.Configs;
using Sernager.Core.Helpers;
using Sernager.Core.Options;
using Sernager.Core.Utils;
using System.ComponentModel;

namespace Sernager.Core.Tests.Units.Configs;

public class ConfigurationMetadataSuccessTests
{
    [DatapointSource]
    private static readonly EConfigurationType[] CONFIGURATION_TYPES = Enum.GetValues<EConfigurationType>();
    [DatapointSource]
    private static readonly EUserFriendlyConfigurationType[] USER_FRIENDLY_CONFIGURATION_TYPES = Enum.GetValues<EUserFriendlyConfigurationType>();

    [Theory]
    public void Parse_ShouldReturnConfigurationMetadata(EConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        byte[] bytes = CaseUtil.Read("Configs.Defaults.Sernager", Configurator.GetExtension(type));

        using (ByteReader reader = new ByteReader(bytes))
        {
            ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type);

            Assert.That(metadata.Config, Is.Not.Null);
            Assert.That(metadata.Config, Is.TypeOf<Configuration>());
        }
    }

    [Theory]
    public void Parse_ShouldReturnConfigurationMetadata(EUserFriendlyConfigurationType ufType)
    {
        Assume.That(ufType, Is.AnyOf(USER_FRIENDLY_CONFIGURATION_TYPES));

        byte[] ufBytes = CaseUtil.Read("Configs.UserFriendlys.Sernager", Configurator.GetExtension(ufType));

        EConfigurationType type = ufType switch
        {
            EUserFriendlyConfigurationType.Yaml => EConfigurationType.Yaml,
            EUserFriendlyConfigurationType.Json => EConfigurationType.Json,
            _ => throw new InvalidEnumArgumentException(nameof(ufType), (int)ufType, typeof(EUserFriendlyConfigurationType))
        };

        using (ByteReader reader = new ByteReader(ufBytes))
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

        byte[] bytes = CaseUtil.Read("Configs.Defaults.Sernager", Configurator.GetExtension(type));

        using (ByteReader reader = new ByteReader(bytes))
        {
            ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type);

            byte[] result = metadata.ToBytes(type);

            if (type != EConfigurationType.Sernager)
            {
                Assert.That(result, Is.Not.Empty);
                Assert.That(result, Is.EqualTo(bytes));

                return;
            }

            string expectJson = JsonWrapper.Serialize(metadata.Config);

            using (ByteReader resultReader = new ByteReader(result))
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

        byte[] ufBytes = CaseUtil.Read("Configs.UserFriendlys.Sernager", Configurator.GetExtension(ufType));

        EConfigurationType type = ufType switch
        {
            EUserFriendlyConfigurationType.Yaml => EConfigurationType.Yaml,
            EUserFriendlyConfigurationType.Json => EConfigurationType.Json,
            _ => throw new InvalidEnumArgumentException(nameof(ufType), (int)ufType, typeof(EUserFriendlyConfigurationType))
        };

        using (ByteReader reader = new ByteReader(ufBytes))
        {
            ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type);

            byte[] result = metadata.ToBytes(ufType);

            Assert.That(EncodingHelper.GetString(result), Is.EqualTo(EncodingHelper.GetString(ufBytes)));

            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Is.EqualTo(ufBytes));
        }
    }
}
