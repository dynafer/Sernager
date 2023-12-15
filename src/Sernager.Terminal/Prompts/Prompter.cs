using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts;

internal sealed class Prompter<T>
{
    private readonly TextWriter mWriter = Console.Out;
    private readonly IPromptComponent mComponent;

    internal Prompter(IPromptComponent component)
    {
        mComponent = component;
    }
}
