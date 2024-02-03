using System.Diagnostics.CodeAnalysis;

namespace Sernager.Terminal.Prompts.Components;

[ExcludeFromCodeCoverage]
internal sealed class LineBreakComponent : IPromptComponent
{
    public bool IsLineBreak { get; } = true;

    string IPromptComponent.Render()
    {
        return string.Empty;
    }
}
