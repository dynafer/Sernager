using Sernager.Core.Options;
using Sernager.Core.Utils;

namespace Sernager.Core.Tests.Units.Utils;

internal sealed class SernagerBuilderUtilTests
{
    [Test]
    public void GetConfigurationTypeOrNull_ShouldReturnNull_WhenPassedNullFilePath()
    {
        EConfigurationType? type = SernagerBuilderUtil.GetConfigurationTypeOrNull(null!);

        Assert.That(type, Is.Null);
    }

    [Test]
    public void GetConfigurationTypeOrNull_ShouldReturnNull_WhenPassedEmptyFilePath()
    {
        EConfigurationType? type = SernagerBuilderUtil.GetConfigurationTypeOrNull(string.Empty);

        Assert.That(type, Is.Null);
    }

    [Test]
    public void GetConfigurationTypeOrNull_ShouldReturnNull_WhenPassedWhiteSpaceFilePath()
    {
        EConfigurationType? type = SernagerBuilderUtil.GetConfigurationTypeOrNull(" ");

        Assert.That(type, Is.Null);
    }

    [Test]
    public void GetConfigurationTypeOrNull_ShouldReturnNull_WhenPassedInvalidExtension()
    {
        EConfigurationType? type = SernagerBuilderUtil.GetConfigurationTypeOrNull("invalid");

        Assert.That(type, Is.Null);
    }

    [Test]
    public void GetConfigurationTypeOrNull_ShouldReturnYaml_WhenPassedYmlExtension()
    {
        EConfigurationType? type = SernagerBuilderUtil.GetConfigurationTypeOrNull("file.yml");

        Assert.That(type, Is.EqualTo(EConfigurationType.Yaml));
    }

    [Test]
    public void GetConfigurationTypeOrNull_ShouldReturnYaml_WhenPassedYamlExtension()
    {
        EConfigurationType? type = SernagerBuilderUtil.GetConfigurationTypeOrNull("file.yaml");

        Assert.That(type, Is.EqualTo(EConfigurationType.Yaml));
    }

    [Test]
    public void GetConfigurationTypeOrNull_ShouldReturnJson_WhenPassedJsonExtension()
    {
        EConfigurationType? type = SernagerBuilderUtil.GetConfigurationTypeOrNull("file.json");

        Assert.That(type, Is.EqualTo(EConfigurationType.Json));
    }

    [Test]
    public void GetConfigurationTypeOrNull_ShouldReturnSernager_WhenPassedSrnExtension()
    {
        EConfigurationType? type = SernagerBuilderUtil.GetConfigurationTypeOrNull("file.srn");

        Assert.That(type, Is.EqualTo(EConfigurationType.Sernager));
    }

    [Test]
    public void GetConfigurationTypeOrNull_ShouldReturnNull_WhenPassedInvalidExtensionWithDot()
    {
        EConfigurationType? type = SernagerBuilderUtil.GetConfigurationTypeOrNull(".invalid");

        Assert.That(type, Is.Null);
    }
}
