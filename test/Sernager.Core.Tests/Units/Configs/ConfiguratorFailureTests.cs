using Sernager.Core.Configs;
using Sernager.Core.Options;
using System.ComponentModel;

namespace Sernager.Core.Tests.Units.Configs;

internal sealed class ConfiguratorFailureTests : FailureFixture
{
    [DatapointSource]
    private static readonly EConfigurationType[] CONFIGURATION_TYPES = Enum.GetValues<EConfigurationType>();
    [DatapointSource]
    private static readonly EUserFriendlyConfigurationType[] USER_FRIENDLY_CONFIGURATION_TYPES = Enum.GetValues<EUserFriendlyConfigurationType>();

    [TearDown]
    public void ResetConfigurator()
    {
        ResetUtil.ResetConfigurator();
    }

    [Test]
    public void Init_ShouldThrow_WhenConfiguratorAlreadyInitialized()
    {
        Configurator.Init();

        TestNoneLevel(Configurator.Init);
        TestExceptionLevel<InvalidOperationException>(Configurator.Init);
    }

    [Test]
    public void Parse_ShouldThrow_WhenConfiguratorAlreadyInitialized()
    {
        Configurator.Init();

        TestNoneLevel(() => Configurator.Parse("path/to/file"));
        TestExceptionLevel<InvalidOperationException>(() => Configurator.Parse("path/to/file"));
    }

    [Test]
    public void Parse_ShouldThrow_WhenInvalidConfigurationFileExtension()
    {
        string filePath = "path/to/file";

        TestNoneLevel(() => Configurator.Parse(filePath));
        TestExceptionLevel<InvalidOperationException>(() => Configurator.Parse(filePath));
    }

    [Theory]
    public void SaveAsFile_ShouldThrow_WhenConfiguratorNotInitialized(EConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        TestNoneLevel(() => Configurator.SaveAsFile(type));
        TestExceptionLevel<InvalidOperationException>(() => Configurator.SaveAsFile(type));
    }

    [Theory]
    public void SaveAsFile_ShouldThrow_WhenConfiguratorNotInitialized(EUserFriendlyConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(USER_FRIENDLY_CONFIGURATION_TYPES));

        TestNoneLevel(() => Configurator.SaveAsFile(type));
        TestExceptionLevel<InvalidOperationException>(() => Configurator.SaveAsFile(type));
    }

    [Test]
    public void GetExtension_ShouldThrow_WhenConfigurationTypeDoesNotExist()
    {
        Assert.Throws<InvalidEnumArgumentException>(() => Configurator.GetExtension((EConfigurationType)int.MaxValue));
    }

    [Test]
    public void GetExtension_ShouldThrow_WhenUserFriendlyConfigurationTypeDoesNotExist()
    {
        Assert.Throws<InvalidEnumArgumentException>(() => Configurator.GetExtension((EUserFriendlyConfigurationType)int.MaxValue));
    }
}
