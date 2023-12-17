namespace Sernager.Terminal.Prompts;

internal sealed class PromptChoice
{
    internal string Name { get; init; }
    internal string Value { get; init; }
    internal bool IsSelected { get; set; } = false;

    internal PromptChoice(string name)
    {
        Name = name;
        Value = name;
    }

    internal PromptChoice(string name, string value)
    {
        Name = name;
        Value = value;
    }
}
