namespace Sernager.Core.Models;

internal sealed class UserFriendlyGroupModel
{
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<object> Items { get; init; } = new List<object>();
}
