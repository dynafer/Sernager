namespace Sernager.Core.Models;

public sealed class GroupModel
{
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Guid> Items { get; init; } = new List<Guid>();
}
