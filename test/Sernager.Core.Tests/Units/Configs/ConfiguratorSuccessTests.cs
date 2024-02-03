using Sernager.Core.Configs;
using Sernager.Core.Options;
using System.ComponentModel;

namespace Sernager.Core.Tests.Units.Configs;

internal sealed class ConfiguratorSuccessTests
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
    public void Init_ShouldInitialize()
    {
        Configurator.Init();

        string expectedConfigDir = Path.GetFullPath(Directory.GetCurrentDirectory());
        string expectedConfigName = "sernager";

        Assert.That(PrivateUtil.GetFieldValue<string>(typeof(Configurator), "mConfigDir"), Is.EqualTo(expectedConfigDir));
        Assert.That(PrivateUtil.GetFieldValue<string>(typeof(Configurator), "mConfigName"), Is.EqualTo(expectedConfigName));
        Assert.That(Configurator.IsInitialized, Is.True);
        Assert.That(Configurator.Config, Is.Not.Null);
        Assert.That(Configurator.Config, Is.TypeOf<Configuration>());
    }

    [Theory]
    public void Parse_ShouldInitialize(EConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        string filePath = CaseUtil.GetPath("Configs.Defaults.Sernager", Configurator.GetExtension(type));

        Configurator.Parse(filePath);

        string expectedConfigDir = Path.GetFullPath(Path.GetDirectoryName(filePath) ?? "./");
        string expectedConfigName = Path.GetFileNameWithoutExtension(filePath)
            .Replace(".default", "")
            .Replace(".userfriendly", "");

        Assert.That(PrivateUtil.GetFieldValue<string>(typeof(Configurator), "mConfigDir"), Is.EqualTo(expectedConfigDir));
        Assert.That(PrivateUtil.GetFieldValue<string>(typeof(Configurator), "mConfigName"), Is.EqualTo(expectedConfigName));
        Assert.That(Configurator.IsInitialized, Is.True);
        Assert.That(Configurator.Config, Is.Not.Null);
        Assert.That(Configurator.Config, Is.TypeOf<Configuration>());
    }

    [Theory]
    public void Parse_ShouldInitialize_WhenPassedOnlyFile(EConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        string filePath = CaseUtil.GetPath("Configs.Defaults.Sernager", Configurator.GetExtension(type));
        string copiedPath = Path.Combine(Path.GetFullPath("./"), $"def.{Path.GetFileName(filePath)}");

        File.Copy(filePath, copiedPath, true);

        Configurator.Parse(Path.GetFileName(copiedPath));

        string expectedConfigDir = Path.GetFullPath("./");
        string expectedConfigName = Path.GetFileNameWithoutExtension(copiedPath)
            .Replace(".default", "")
            .Replace(".userfriendly", "");

        Assert.That(PrivateUtil.GetFieldValue<string>(typeof(Configurator), "mConfigDir"), Is.EqualTo(expectedConfigDir));
        Assert.That(PrivateUtil.GetFieldValue<string>(typeof(Configurator), "mConfigName"), Is.EqualTo(expectedConfigName));
        Assert.That(Configurator.IsInitialized, Is.True);
        Assert.That(Configurator.Config, Is.Not.Null);
        Assert.That(Configurator.Config, Is.TypeOf<Configuration>());

        File.Delete(copiedPath);
    }

    [Theory]
    public void Parse_ShouldInitialize(EUserFriendlyConfigurationType ufType)
    {
        Assume.That(ufType, Is.AnyOf(USER_FRIENDLY_CONFIGURATION_TYPES));

        string filePath = CaseUtil.GetPath("Configs.UserFriendlys.Sernager", Configurator.GetExtension(ufType));

        Configurator.Parse(filePath);

        string expectedConfigDir = Path.GetFullPath(Path.GetDirectoryName(filePath) ?? "./");
        string expectedConfigName = Path.GetFileNameWithoutExtension(filePath)
            .Replace(".default", "")
            .Replace(".userfriendly", "");

        Assert.That(PrivateUtil.GetFieldValue<string>(typeof(Configurator), "mConfigDir"), Is.EqualTo(expectedConfigDir));
        Assert.That(PrivateUtil.GetFieldValue<string>(typeof(Configurator), "mConfigName"), Is.EqualTo(expectedConfigName));
        Assert.That(Configurator.IsInitialized, Is.True);
        Assert.That(Configurator.Config, Is.Not.Null);
        Assert.That(Configurator.Config, Is.TypeOf<Configuration>());
    }

    [Theory]
    public void Parse_ShouldInitialize_WhenPassedOnlyFile(EUserFriendlyConfigurationType ufType)
    {
        Assume.That(ufType, Is.AnyOf(USER_FRIENDLY_CONFIGURATION_TYPES));

        string filePath = CaseUtil.GetPath("Configs.UserFriendlys.Sernager", Configurator.GetExtension(ufType));
        string copiedPath = Path.Combine(Path.GetFullPath("./"), $"uf.{Path.GetFileName(filePath)}");

        File.Copy(filePath, copiedPath, true);

        Configurator.Parse(Path.GetFileName(copiedPath));

        string expectedConfigDir = Path.GetFullPath("./");
        string expectedConfigName = Path.GetFileNameWithoutExtension(copiedPath)
            .Replace(".default", "")
            .Replace(".userfriendly", "");

        Assert.That(PrivateUtil.GetFieldValue<string>(typeof(Configurator), "mConfigDir"), Is.EqualTo(expectedConfigDir));
        Assert.That(PrivateUtil.GetFieldValue<string>(typeof(Configurator), "mConfigName"), Is.EqualTo(expectedConfigName));
        Assert.That(Configurator.IsInitialized, Is.True);
        Assert.That(Configurator.Config, Is.Not.Null);
        Assert.That(Configurator.Config, Is.TypeOf<Configuration>());

        File.Delete(copiedPath);
    }

    [Theory]
    public void SaveAsFile_ShouldSave(EConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        string filePath = CaseUtil.GetPath("Configs.Defaults.Sernager", Configurator.GetExtension(type));

        Configurator.Parse(filePath);

        string expectedConfigDir = Path.GetFullPath(Path.GetDirectoryName(filePath) ?? "./");
        string expectedConfigName = Path.GetFileNameWithoutExtension(filePath)
            .Replace(".default", "")
            .Replace(".userfriendly", "");

        Assert.That(PrivateUtil.GetFieldValue<string>(typeof(Configurator), "mConfigDir"), Is.EqualTo(expectedConfigDir));
        Assert.That(PrivateUtil.GetFieldValue<string>(typeof(Configurator), "mConfigName"), Is.EqualTo(expectedConfigName));
        Assert.That(Configurator.IsInitialized, Is.True);
        Assert.That(Configurator.Config, Is.Not.Null);
        Assert.That(Configurator.Config, Is.TypeOf<Configuration>());

        Configurator.SaveAsFile(type);

        string expectedFilePath = Path.Combine(expectedConfigDir, $"{expectedConfigName}.default{Configurator.GetExtension(type)}");

        Assert.That(File.Exists(expectedFilePath), Is.True);

        File.Delete(expectedFilePath);
    }

    [Theory]
    public void SaveAsFile_ShouldSave(EUserFriendlyConfigurationType ufType)
    {
        Assume.That(ufType, Is.AnyOf(USER_FRIENDLY_CONFIGURATION_TYPES));

        string filePath = CaseUtil.GetPath("Configs.UserFriendlys.Sernager", Configurator.GetExtension(ufType));

        Configurator.Parse(filePath);

        string expectedConfigDir = Path.GetFullPath(Path.GetDirectoryName(filePath) ?? "./");
        string expectedConfigName = Path.GetFileNameWithoutExtension(filePath)
            .Replace(".default", "")
            .Replace(".userfriendly", "");

        Assert.That(PrivateUtil.GetFieldValue<string>(typeof(Configurator), "mConfigDir"), Is.EqualTo(expectedConfigDir));
        Assert.That(PrivateUtil.GetFieldValue<string>(typeof(Configurator), "mConfigName"), Is.EqualTo(expectedConfigName));
        Assert.That(Configurator.IsInitialized, Is.True);
        Assert.That(Configurator.Config, Is.Not.Null);
        Assert.That(Configurator.Config, Is.TypeOf<Configuration>());

        Configurator.SaveAsFile(ufType);

        string expectedFilePath = Path.Combine(expectedConfigDir, $"{expectedConfigName}.userfriendly{Configurator.GetExtension(ufType)}");

        Assert.That(File.Exists(expectedFilePath), Is.True);

        File.Delete(expectedFilePath);
    }

    [Theory]
    public void GetExtension_ShouldReturnExtension(EConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        string result = Configurator.GetExtension(type);
        string expectedExetnsion = type switch
        {
            EConfigurationType.Json => ".json",
            EConfigurationType.Yaml => ".yaml",
            EConfigurationType.Sernager => ".srn",
            _ => throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(EConfigurationType))
        };

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.EqualTo(expectedExetnsion));
    }

    [Theory]
    public void GetExtension_ShouldReturnExtension(EUserFriendlyConfigurationType ufType)
    {
        Assume.That(ufType, Is.AnyOf(USER_FRIENDLY_CONFIGURATION_TYPES));

        string result = Configurator.GetExtension(ufType);
        string expectedExetnsion = ufType switch
        {
            EUserFriendlyConfigurationType.Json => ".json",
            EUserFriendlyConfigurationType.Yaml => ".yaml",
            _ => throw new InvalidEnumArgumentException(nameof(ufType), (int)ufType, typeof(EUserFriendlyConfigurationType))
        };

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.EqualTo(expectedExetnsion));
    }
}
