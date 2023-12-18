using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Factories.Plugins;

internal sealed class InputPlugin : IBasePlugin
{
    public string Prompt { get; set; } = string.Empty;
    private AutoComplete<string>? mAutoComplete = null;

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

    List<IStyledComponent> IBasePlugin.Render()
    {
        List<IStyledComponent> components = new List<IStyledComponent>();

        return components;
    }
}
