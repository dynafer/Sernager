namespace Sernager.Terminal.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class FlowAttribute : Attribute
{
    public string Name { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;

    public FlowAttribute()
    {
    }

    public FlowAttribute(string alias)
    {
        Alias = alias;
    }

    public FlowAttribute(string name, string alias)
    {
        Name = name;
        Alias = alias;
    }
}
