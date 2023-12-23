namespace Sernager.Terminal.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ArgAttribute : Attribute
{
    public required string Name { get; set; }
    public string ShortName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsBool { get; set; } = false;
}
