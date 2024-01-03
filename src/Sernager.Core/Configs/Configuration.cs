using Sernager.Core.Models;

namespace Sernager.Core.Configs;

internal sealed class Configuration
{
    public Dictionary<string, Dictionary<string, string>> Settings { get; init; } = new Dictionary<string, Dictionary<string, string>>();
    public Dictionary<string, GroupModel> Groups { get; init; } = new Dictionary<string, GroupModel>();
    public Dictionary<Guid, GroupModel> SubGroups { get; init; } = new Dictionary<Guid, GroupModel>();
    public Dictionary<Guid, CommandModel> Commands { get; init; } = new Dictionary<Guid, CommandModel>();
}
