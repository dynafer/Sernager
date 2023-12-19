using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Factories.Plugins;

internal sealed class InputPlugin : IBasePlugin
{
    private AutoComplete<string>? mAutoComplete = null;
    public string Prompt { get; set; } = string.Empty;

    internal InputPlugin UseAutoComplete()
    {
        mAutoComplete = new AutoComplete<string>();

        return this;
    }

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo)
    {
        return true;
    }

    List<IPromptComponent> IBasePlugin.Render()
    {
        List<IPromptComponent> components = new List<IPromptComponent>();

        return components;
    }
}
