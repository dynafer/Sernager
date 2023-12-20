using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Factories.Plugins;

internal sealed class ConfirmPlugin : IBasePlugin
{
    public string Prompt { get; set; } = string.Empty;

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo, out object result)
    {
        result = null!;
        return true;
    }

    List<IPromptComponent> IBasePlugin.Render()
    {
        List<IPromptComponent> components = new List<IPromptComponent>();

        return components;
    }
}
