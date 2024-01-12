namespace Sernager.Core.Tests.Fakes;

internal sealed class FakeModel
{
    public string Name { get; set; } = string.Empty;
    public int Number { get; set; }
    public bool IsEnabled { get; set; }
    public string CaseName { get; set; } = string.Empty;
    public string? NullName { get; set; }
}
