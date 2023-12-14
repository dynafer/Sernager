namespace Sernager.Core.Configs;

internal sealed class Configuration
{
    internal List<string> SettingNames { get; init; } = new List<string>();
    internal Dictionary<string, Dictionary<string, string>> Settings { get; init; } = new Dictionary<string, Dictionary<string, string>>();
}
