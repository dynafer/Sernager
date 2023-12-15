namespace Sernager.Terminal.Prompts;

internal readonly record struct PromptChoice
{
    internal string Title { get; init; }
    internal string Value { get; init; }

    internal PromptChoice(string title)
    {
        Title = title;
        Value = title;
    }

    internal PromptChoice(string title, string value)
    {
        Title = title;
        Value = value;
    }
}
