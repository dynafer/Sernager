using Sernager.Core.Models;

namespace Sernager.Core.Configs;

internal sealed class Configuration
{
    public Dictionary<string, EnvironmentModel> EnvironmentGroups { get; init; } = new Dictionary<string, EnvironmentModel>();
    public Dictionary<string, GroupModel> CommandMainGroups { get; init; } = new Dictionary<string, GroupModel>();
    public Dictionary<Guid, GroupModel> CommandSubGroups { get; init; } = new Dictionary<Guid, GroupModel>();
    public Dictionary<Guid, CommandModel> Commands { get; init; } = new Dictionary<Guid, CommandModel>();
}
