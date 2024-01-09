using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Cursors;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;

namespace Sernager.Terminal.Prompts.Plugins;

internal sealed class ConfirmPlugin : ITypePlugin<bool>
{
    private string mResult = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public List<string> Description { get; private init; } = new List<string>();
    public bool ShouldShowCursor => false;

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

        if (Description.Count > 0)
        {
            components.Add(new LineBreakComponent());
            components.AddRange(
                Description
                    .Select((string description) =>
                    {
                        return new TextComponent()
                            .SetTextColor(EColorFlags.BrightBlack)
                            .SetText(description)
                            .UseLineBreak();
                    })
            );
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
                .AddCursor(ECursorDirection.Right, 1),
            new TextComponent()
                .SetDecoration(EDecorationFlags.Bold)
                .SetTextColor(EColorFlags.Green)
                .SetText(mResult)
                .UseLineBreak(),
        ];

        return components;
    }
}
