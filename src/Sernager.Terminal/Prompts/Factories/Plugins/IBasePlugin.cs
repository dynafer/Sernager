using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Factories.Plugins;

internal interface IBasePlugin
{
    string Prompt { get; }
    List<IPromptComponent> Render();
}
