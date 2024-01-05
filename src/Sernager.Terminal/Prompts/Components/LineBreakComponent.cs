namespace Sernager.Terminal.Prompts.Components;

internal sealed class LineBreakComponent : IPromptComponent
{
    public bool IsLineBreak { get; } = true;

    string IPromptComponent.Render()
    {
        return string.Empty;
    }
}
