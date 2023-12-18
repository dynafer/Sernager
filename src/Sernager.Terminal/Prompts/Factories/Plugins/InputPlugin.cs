using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Factories.Plugins;

internal sealed class InputPlugin : IBasePlugin
{
    private AutoComplete<string>? mAutoComplete = null;
    public string Prompt { get; private set; } = string.Empty;

    internal InputPlugin SetPrompt(string prompt)
    {
        Prompt = prompt;

        return this;
    }

    internal InputPlugin UseAutoComplete()
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
