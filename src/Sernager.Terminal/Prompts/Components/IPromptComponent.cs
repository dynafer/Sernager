namespace Sernager.Terminal.Prompts.Components;

internal interface IPromptComponent
{
    bool IsLineBreak { get; }
    string Render();
}
