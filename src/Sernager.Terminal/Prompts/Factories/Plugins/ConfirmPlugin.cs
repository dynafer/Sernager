using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Factories.Plugins;

internal sealed class ConfirmPlugin : IBasePlugin
{
    public string Prompt { get; set; } = string.Empty;

    internal ConfirmPlugin SetPrompt(string prompt)
    {
        Prompt = prompt;

        return this;
    }

    List<IStyledComponent> IBasePlugin.Render()
    {
        List<IStyledComponent> components = new List<IStyledComponent>();

        return components;
    }
}
