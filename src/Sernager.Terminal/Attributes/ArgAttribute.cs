using System.Diagnostics.CodeAnalysis;

namespace Sernager.Terminal.Attributes;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ArgAttribute : Attribute
{
    public required string Name { get; set; }
    public string ShortName { get; set; } = string.Empty;
    public string DescriptionResourceName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
