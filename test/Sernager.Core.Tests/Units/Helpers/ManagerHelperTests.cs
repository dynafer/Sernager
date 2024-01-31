using Sernager.Core.Configs;
using Sernager.Core.Helpers;
using Sernager.Core.Models;
using Sernager.Core.Options;
using System.Diagnostics;

namespace Sernager.Core.Tests.Units.Helpers;

public class ManagerHelperTests : FailureFixture
{
    [DatapointSource]
    private static readonly EConfigurationType[] CONFIGURATION_TYPES = Enum.GetValues<EConfigurationType>();

    [TearDown]
    public void ResetConfigurator()
    {
        ResetUtil.ResetConfigurator();
    }

    [Theory]
    public void CanUseCommandMainGroupName_ShouldReturnTrue(EConfigurationType type)
    {
        initializeConfigurator(type);

        string name = "inexistentName";
        string shortName = "inexistentShortName";

        Assert.That(ManagerHelper.CanUseCommandMainGroupName(name), Is.True);
        Assert.That(ManagerHelper.CanUseCommandMainGroupName(shortName), Is.True);
    }

    [Theory]
    public void CanUseCommandMainGroupName_ShouldReturnFalse(EConfigurationType type)
    {
        initializeConfigurator(type);

        GroupModel existedGroup = Configurator.Config.CommandMainGroups.Values.First();

        Assert.That(ManagerHelper.CanUseCommandMainGroupName(existedGroup.Name), Is.False);
        Assert.That(ManagerHelper.CanUseCommandMainGroupName(existedGroup.ShortName), Is.False);
    }

    [Theory]
    public void CanUseEnvironmentGroupName_ShouldReturnTrue(EConfigurationType type)
    {
        initializeConfigurator(type);

        string name = "inexistentName";

        Assert.That(ManagerHelper.CanUseEnvironmentGroupName(name), Is.True);
    }

    [Theory]
    public void CanUseEnvironmentGroupName_ShouldReturnFalse(EConfigurationType type)
    {
        initializeConfigurator(type);

        EnvironmentModel existedGroup = Configurator.Config.EnvironmentGroups.Values.First();

        Assert.That(ManagerHelper.CanUseEnvironmentGroupName(existedGroup.Name), Is.False);
    }

    [Theory]
    public void GetCommadMainGroupNameOrNull_ShouldReturnName(EConfigurationType type)
    {
        initializeConfigurator(type);

        GroupModel existedGroup = Configurator.Config.CommandMainGroups.Values.First();

        Assert.That(ManagerHelper.GetCommadMainGroupNameOrNull(existedGroup.Name), Is.EqualTo(existedGroup.Name));
        Assert.That(ManagerHelper.GetCommadMainGroupNameOrNull(existedGroup.ShortName), Is.EqualTo(existedGroup.Name));
    }

    [Theory]
    public void GetCommadMainGroupNameOrNull_ShouldReturnNull(EConfigurationType type)
    {
        initializeConfigurator(type);

        string name = "inexistentName";
        string shortName = "inexistentShortName";

        Assert.That(ManagerHelper.GetCommadMainGroupNameOrNull(name), Is.Null);
        Assert.That(ManagerHelper.GetCommadMainGroupNameOrNull(shortName), Is.Null);
    }

    [Theory]
    public void IsCommand_ShouldReturnTrue(EConfigurationType type)
    {
        initializeConfigurator(type);

        Guid id = Configurator.Config.Commands.Keys.First();

        Assert.That(ManagerHelper.IsCommand(id), Is.True);
    }

    [Theory]
    public void IsCommand_ShouldReturnFalse(EConfigurationType type)
    {
        initializeConfigurator(type);

        Guid id = Guid.NewGuid();

        Assert.That(ManagerHelper.IsCommand(id), Is.False);
    }

    [Theory]
    public void GetCommandOrNull_ShouldReturnCommand(EConfigurationType type)
    {
        initializeConfigurator(type);

        Guid id = Configurator.Config.Commands.Keys.First();

        Assert.That(ManagerHelper.GetCommandOrNull(id), Is.EqualTo(Configurator.Config.Commands[id]));
    }

    [Theory]
    public void GetCommandOrNull_ShouldThrow(EConfigurationType type)
    {
        initializeConfigurator(type);

        Guid id = Guid.NewGuid();

        TestNoneLevel(() => ManagerHelper.GetCommandOrNull(id), Is.Null);
        TestExceptionLevel<SernagerException>(() => ManagerHelper.GetCommandOrNull(id));
    }

    [StackTraceHidden]
    private void initializeConfigurator(EConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        string filePath = CaseUtil.GetPath("Configs.Defaults.Sernager", Configurator.GetExtension(type));

        Configurator.Parse(filePath);
    }
}
