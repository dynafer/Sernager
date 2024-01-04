namespace Sernager.Core.Models;

public sealed class EnvironmentModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public string ShortName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Dictionary<string, string> PreVariables { get; init; } = new Dictionary<string, string>();
    public Dictionary<string, string> Variables { get; init; } = new Dictionary<string, string>();
}
