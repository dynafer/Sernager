namespace Sernager.Core.Models;

internal sealed class GroupModel
{
    public string Name { get; init; } = string.Empty;
    public string ShortName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<Guid> Items { get; init; } = new List<Guid>();
}
