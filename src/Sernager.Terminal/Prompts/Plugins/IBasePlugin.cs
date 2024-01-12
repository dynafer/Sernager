using Sernager.Resources;
using Sernager.Terminal.Prompts.Components;
using System.Diagnostics.CodeAnalysis;

namespace Sernager.Terminal.Prompts.Plugins;

internal interface IBasePlugin
{
    IResourcePack? ResourcePack { get; set; }
    string Prompt { get; set; }
    List<string> Description { get; }
    bool ShouldShowCursor { get; }
    bool Input(ConsoleKeyInfo keyInfo, [NotNullWhen(true)] out object? result);
    List<IPromptComponent> Render();
    List<IPromptComponent> RenderLast();
}
