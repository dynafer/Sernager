namespace Sernager.Core.Models;

public sealed class EnvironmentModel
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> SubstVariables { get; init; } = new Dictionary<string, string>();
    public Dictionary<string, string> Variables { get; init; } = new Dictionary<string, string>();
}
