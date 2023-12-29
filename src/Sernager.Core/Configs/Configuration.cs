namespace Sernager.Core.Configs;

internal sealed class Configuration
{
    internal Dictionary<string, Dictionary<string, string>> Settings { get; init; } = new Dictionary<string, Dictionary<string, string>>();
}
