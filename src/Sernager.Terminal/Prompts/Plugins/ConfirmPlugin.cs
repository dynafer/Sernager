using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Cursors;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;

namespace Sernager.Terminal.Prompts.Plugins;

internal sealed class ConfirmPlugin : ITypePlugin<bool>
{
    private string mResult = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo, out object result)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Y:
            case ConsoleKey.N:
                result = keyInfo.Key == ConsoleKey.Y;
                mResult = keyInfo.Key.ToString();

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

        if (!string.IsNullOrWhiteSpace(Description))
        {
            components.AddRange([
                new LineBreakComponent(),
                new TextComponent()
                    .SetTextColor(EColorFlags.BrightBlack)
                    .SetText(Description)
            ]);
        }

        return components;
    }

    List<IPromptComponent> IBasePlugin.RenderLast()
    {
        List<IPromptComponent> components =
        [
            new TextComponent()
                .SetDecoration(EDecorationFlags.Bold)
                .SetText(Prompt),
            new CursorComponent()
                .AddCursors(
                    new { Direction = ECursorDirection.Right, Count = 1 }
                ),
            new TextComponent()
                .SetDecoration(EDecorationFlags.Bold)
                .SetTextColor(EColorFlags.Green)
                .SetText(mResult)
                .UseLineBreak(),
        ];

        return components;
    }
}
