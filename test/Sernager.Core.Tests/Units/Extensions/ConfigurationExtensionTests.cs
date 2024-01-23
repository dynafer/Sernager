using Sernager.Core.Configs;
using Sernager.Core.Extensions;
using Sernager.Core.Models;
using Sernager.Core.Options;
using Sernager.Unit.Extensions;
using System.ComponentModel;
using System.Text;

namespace Sernager.Core.Tests.Units.Extensions;

public class ConfigurationExtensionTests
{
    [DatapointSource]
    private static readonly EConfigurationType[] CONFIGURATION_TYPES = Enum.GetValues<EConfigurationType>();
    [DatapointSource]
    private static readonly Encoding[] ENCODING_LIST =
    [
        Encoding.UTF8,
        Encoding.Unicode,
        Encoding.BigEndianUnicode,
        Encoding.UTF32,
        Encoding.ASCII,
    ];

    public void ToUserFriendlyConfiguration_ShouldReturnUserFriendlyConfiguration()
    {
        Guid subgroupId = Guid.NewGuid();
        Guid commandId1 = Guid.NewGuid();
        Guid commandId2 = Guid.NewGuid();

        EnvironmentModel envModel = new EnvironmentModel
        {
            Name = "env",
            PreVariables = new Dictionary<string, string>
            {
                { "PRE_ENV", "PRE_ENV" }
            },
            Variables = new Dictionary<string, string>
            {
                { "ENV", "ENV" }
            }
        };

        GroupModel mainGroupModel = new GroupModel
        {
            Name = "group",
            ShortName = "g",
            Description = "group description",
            Items = new List<Guid>
            {
                commandId1,
                subgroupId
            }
        };

        GroupModel subgroupModel = new GroupModel
        {
            Name = "subgroup",
            ShortName = "sg",
            Description = "subgroup description",
            Items = new List<Guid>
            {
                commandId2
            }
        };

        CommandModel commandModel1 = new CommandModel
        {
            Name = "command1",
            ShortName = "c1",
            Description = "command1 description",
            UsedEnvironmentGroups = new List<string> { "env" },
            Command = new string[] { "command1" },
        };

        CommandModel commandModel2 = new CommandModel
        {
            Name = "command2",
            ShortName = "c2",
            Description = "command2 description",
            UsedEnvironmentGroups = new List<string> { "env" },
            Command = "command2"
        };

        Configuration config = new Configuration();

        config.EnvironmentGroups.Add("env", envModel);
        config.CommandMainGroups.Add("group", mainGroupModel);
        config.CommandSubgroups.Add(subgroupId, subgroupModel);
        config.Commands.Add(commandId1, commandModel1);
        config.Commands.Add(commandId2, commandModel2);

        UserFriendlyConfiguration ufConfig = config.ToUserFriendlyConfiguration();

        Assert.That(ufConfig.Environments.Keys.Count, Is.EqualTo(1));
        Assert.That(ufConfig.Commands.Keys.Count, Is.EqualTo(1));

        Assert.That(ufConfig.Environments["env"], Is.EqualTo(envModel));

        Assert.That(ufConfig.Commands["group"].Name, Is.EqualTo("group"));
        Assert.That(ufConfig.Commands["group"].ShortName, Is.EqualTo("g"));
        Assert.That(ufConfig.Commands["group"].Description, Is.EqualTo("group description"));
        Assert.That(ufConfig.Commands["group"].Items.Count, Is.EqualTo(2));

        Assert.That(ufConfig.Commands["group"].Items[0], Is.EqualTo(commandModel1));

        UserFriendlyGroupModel subgroup = (UserFriendlyGroupModel)ufConfig.Commands["group"].Items[1];
        Assert.That(subgroup.Name, Is.EqualTo("subgroup"));
        Assert.That(subgroup.ShortName, Is.EqualTo("sg"));
        Assert.That(subgroup.Description, Is.EqualTo("subgroup description"));
        Assert.That(subgroup.Items.Count, Is.EqualTo(1));

        Assert.That(subgroup.Items[0], Is.EqualTo(commandModel2));
    }

    [Theory]
    public void ToUserFriendlyConfiguration_ShouldReturnUserFriendlyConfiguration(EConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(CONFIGURATION_TYPES));

        Configuration config = type switch
        {
            EConfigurationType.Yaml => CaseUtil.ReadYaml<Configuration>("Configs.Defaults.Sernager"),
            EConfigurationType.Json => CaseUtil.ReadJson<Configuration>("Configs.Defaults.Sernager"),
            EConfigurationType.Sernager => CaseUtil.ReadSernagerConfig("Configs.Defaults.Sernager"),
            _ => throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(EConfigurationType))
        };

        UserFriendlyConfiguration ufConfig = config.ToUserFriendlyConfiguration();

        Assert.That(ufConfig.Environments.Keys.Count, Is.EqualTo(3));
        Assert.That(ufConfig.Commands.Keys.Count, Is.EqualTo(2));
    }

    [Theory]
    public void ToUserFriendlyConfiguration_ShouldReturnUserFriendlyConfiguration(Encoding encoding)
    {
        Assume.That(encoding, Is.AnyOf(ENCODING_LIST));

        Configuration config = CaseUtil.ReadSernagerConfig($"Configs.Defaults.Sernager.{encoding.GetEncodingName()}");

        UserFriendlyConfiguration ufConfig = config.ToUserFriendlyConfiguration();

        Assert.That(ufConfig.Environments.Keys.Count, Is.EqualTo(3));
        Assert.That(ufConfig.Commands.Keys.Count, Is.EqualTo(2));
    }
}
