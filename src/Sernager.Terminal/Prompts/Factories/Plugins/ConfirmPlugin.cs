using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Factories.Plugins;

internal sealed class ConfirmPlugin : IBasePlugin
{
    public string Prompt { get; private set; } = string.Empty;

    internal ConfirmPlugin SetPrompt(string prompt)
    {
        Prompt = prompt;

        return this;
    }

    List<IPromptComponent> IBasePlugin.Render()
    {
        List<IPromptComponent> components = new List<IPromptComponent>();

        return components;
    }
}
