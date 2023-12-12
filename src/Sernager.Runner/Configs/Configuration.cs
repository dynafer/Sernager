namespace Sernager.Runner.Configs;

public class Configuration
{
    public List<string> SettingNames { get; init; } = new List<string>();
    public Dictionary<string, Dictionary<string, string>> Settings { get; init; } = new Dictionary<string, Dictionary<string, string>>();
}
