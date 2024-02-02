using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Options;

namespace Sernager.Core.Tests.Units;

public class SernagerBuilderTests
{
    [DatapointSource]
    private static readonly EErrorLevel[] ERROR_LEVELS = Enum.GetValues<EErrorLevel>();
    [DatapointSource]
    private static readonly EConfigurationType[] CONFIGURATION_TYPES = Enum.GetValues<EConfigurationType>();
    [DatapointSource]
    private static readonly EUserFriendlyConfigurationType[] USER_FRIENDLY_CONFIGURATION_TYPES = Enum.GetValues<EUserFriendlyConfigurationType>();
    private SernagerBuilder mBuilder;

    [SetUp]
    public void SetUpBuilder()
    {
        mBuilder = new SernagerBuilder();
    }

    [TearDown]
    public void Reset()
    {
        ResetUtil.ResetConfigurator();
        mBuilder = null!;
    }

    [Theory]
    public void SetErrorLevel_ShouldSetErrorLevel(EErrorLevel level)
    {
        Assume.That(level, Is.AnyOf(ERROR_LEVELS));

        mBuilder.SetErrorLevel(level);

        Assert.That(ExceptionManager.ErrorLevel, Is.EqualTo(level));

        ExceptionManager.ErrorLevel = EErrorLevel.None;
    }

    [Test]
    public void UseConfig_ShouldSetConfigFilePath()
    {
        string configPath = "test.json";

        mBuilder.UseConfig(configPath);

        string? expected = PrivateUtil.GetFieldValue<string>(mBuilder, "mConfigFilePath");

        Assert.That(expected, Is.Not.Null);
        Assert.That(expected, Is.EqualTo(configPath));
    }

    [Theory]
    public void Build_ShouldReturnSernagerService(EErrorLevel level, EConfigurationType type)
    {
        Assume.That(level, Is.AnyOf(ERROR_LEVELS));
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        string configPath = CaseUtil.GetPath("Configs.Defaults.Sernager", Configurator.GetExtension(type));

        mBuilder
            .SetErrorLevel(level)
            .UseConfig(configPath);

        string? expectedConfigPath = PrivateUtil.GetFieldValue<string>(mBuilder, "mConfigFilePath");

        Assert.That(mBuilder.Build(), Is.InstanceOf<ISernagerService>());
        Assert.That(ExceptionManager.ErrorLevel, Is.EqualTo(level));
        Assert.That(expectedConfigPath, Is.EqualTo(configPath));
    }

    [Theory]
    public void Build_ShouldReturnSernagerService(EErrorLevel level, EUserFriendlyConfigurationType type)
    {
        Assume.That(level, Is.AnyOf(ERROR_LEVELS));
        Assume.That(type, Is.AnyOf(USER_FRIENDLY_CONFIGURATION_TYPES));

        string configPath = CaseUtil.GetPath("Configs.UserFriendlys.Sernager", Configurator.GetExtension(type));

        mBuilder
            .SetErrorLevel(level)
            .UseConfig(configPath);

        string? expectedConfigPath = PrivateUtil.GetFieldValue<string>(mBuilder, "mConfigFilePath");

        Assert.That(mBuilder.Build(), Is.InstanceOf<ISernagerService>());
        Assert.That(ExceptionManager.ErrorLevel, Is.EqualTo(level));
        Assert.That(expectedConfigPath, Is.EqualTo(configPath));
    }
}
