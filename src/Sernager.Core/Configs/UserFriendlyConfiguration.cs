using Sernager.Core.Models;

namespace Sernager.Core.Configs;

internal sealed class UserFriendlyConfiguration
{
    public Dictionary<string, EnvironmentModel> Environments { get; init; } = new Dictionary<string, EnvironmentModel>();
    public Dictionary<string, UserFriendlyGroupModel> Commands { get; init; } = new Dictionary<string, UserFriendlyGroupModel>();
}
