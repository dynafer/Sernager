namespace Sernager.Terminal.Prompts.Components;

internal interface IStyledComponent
{
    bool IsLineBreak { get; }
    string Render();
}
