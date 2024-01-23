using Sernager.Core.Configs;
using Sernager.Core.Extensions;
using Sernager.Core.Models;
using Sernager.Core.Options;
using Sernager.Core.Utils;
using System.ComponentModel;
using System.Text.Json;

namespace Sernager.Core.Tests.Units.Extensions;

public class UserFriendlyConfigurationExtensionSuccessTests
{
    [DatapointSource]
    private static readonly EUserFriendlyConfigurationType[] USER_FRIENDLY_CONFIGURATION_TYPES = Enum.GetValues<EUserFriendlyConfigurationType>();
    [DatapointSource]
    private static readonly (int, string)[] LEVEL_CASE_PAIRS =
    [
        (1, "OneLevel"),
        (2, "TwoLevels"),
        (3, "ThreeLevels")
    ];

    [Theory]
    public void ToConfiguration_ShouldReturnConfiguration(EUserFriendlyConfigurationType type)
    {
        Assume.That(type, Is.AnyOf(USER_FRIENDLY_CONFIGURATION_TYPES));

        UserFriendlyConfiguration config = type switch
        {
            EUserFriendlyConfigurationType.Json => CaseUtil.ReadJson<UserFriendlyConfiguration>("Configs.UserFriendlys.Sernager"),
            EUserFriendlyConfigurationType.Yaml => CaseUtil.ReadYaml<UserFriendlyConfiguration>("Configs.UserFriendlys.Sernager"),
            _ => throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(EUserFriendlyConfigurationType))
        };

        Configuration configuration = config.ToConfiguration();
        Assert.That(configuration.EnvironmentGroups.Count, Is.EqualTo(config.Environments.Count));
        Assert.That(configuration.CommandMainGroups.Count, Is.EqualTo(config.Commands.Count));
    }

    [Test]
    public void ToConfiguration_ShouldReturnConfiguration_Environments()
    {
        UserFriendlyConfiguration config = CaseUtil.ReadJson<UserFriendlyConfiguration>("Configs.UserFriendlys.Specifications.Environment");

        Configuration configuration = config.ToConfiguration();
        Assert.That(configuration.EnvironmentGroups.Count, Is.EqualTo(config.Environments.Count));
    }

    [Theory]
    public void ToConfiguration_ShouldRetrunConfiguration_Commands((int, string) levelCasePair)
    {
        Assume.That(levelCasePair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = levelCasePair;

        UserFriendlyConfiguration ufConfig = CaseUtil.ReadJson<UserFriendlyConfiguration>($"Configs.UserFriendlys.Specifications.Commands.{caseName}");

        Configuration config = ufConfig.ToConfiguration();
        Assert.That(config.CommandMainGroups.Count, Is.EqualTo(ufConfig.Commands.Count));

        if (level == 1)
        {
            return;
        }

        int sum = 0;
        foreach (KeyValuePair<string, UserFriendlyGroupModel> pair in ufConfig.Commands)
        {
            calculateSum(pair.Value, ref sum);
        }

        Assert.That(sum, Is.EqualTo(config.CommandSubgroups.Count + config.Commands.Count));
    }

    private void calculateSum(object obj, ref int sum)
    {
        if (obj is not UserFriendlyGroupModel groupModel)
        {
            if (obj is not JsonElement jsonElement)
            {
                return;
            }

            UserFriendlyGroupModel? model = JsonWrapper.Deserialize<UserFriendlyGroupModel>(jsonElement.GetRawText());

            if (model == null)
            {
                return;
            }

            groupModel = model;
        }

        sum += groupModel.Items.Count;

        foreach (object item in groupModel.Items)
        {
            calculateSum(item, ref sum);
        }

        return;
    }
}