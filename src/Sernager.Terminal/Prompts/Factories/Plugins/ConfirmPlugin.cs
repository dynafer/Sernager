using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Factories.Plugins;

internal sealed class ConfirmPlugin : IBasePlugin
{
    public string Prompt { get; set; } = string.Empty;

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo, out object result)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Y:
            case ConsoleKey.N:
                result = keyInfo.Key == ConsoleKey.Y;
                return true;
        }

        result = null!;
        return false;
    }

    List<IPromptComponent> IBasePlugin.Render()
    {
        List<IPromptComponent> components =
        [
            new TextComponent()
                .SetDecoration(EDecorationFlags.Bold)
                .SetText($"{Prompt} [Y/n] ")
        ];

        return components;
    }
}
