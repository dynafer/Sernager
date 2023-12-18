using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Factories.Plugins;

internal sealed class SelectionPlugin : IBasePlugin
{
    private AutoComplete<string>? mAutoComplete = null;
    public string Prompt { get; private set; } = string.Empty;

    internal SelectionPlugin SetPrompt(string prompt)
    {
        Prompt = prompt;

        return this;
    }

    internal SelectionPlugin UseAutoComplete()
    {
        mAutoComplete = new AutoComplete<string>();

        return this;
    }

    List<IPromptComponent> IBasePlugin.Render()
    {
        List<IPromptComponent> components = new List<IPromptComponent>();

        return components;
    }
}
