using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Factories.Plugins;

internal interface IBasePlugin
{
    string Prompt { get; set; }
    bool Input(ConsoleKeyInfo keyInfo);
    List<IPromptComponent> Render();
}
