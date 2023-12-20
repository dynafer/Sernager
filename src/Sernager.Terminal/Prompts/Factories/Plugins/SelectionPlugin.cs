using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Cursors;

namespace Sernager.Terminal.Prompts.Factories.Plugins;

internal sealed class SelectionPlugin : IBasePlugin
{
    public readonly Pagination Pagination = new Pagination();
    public readonly List<OptionItem> Options = new List<OptionItem>();
    private AutoComplete<OptionItem>? mAutoComplete = null;
    public string Prompt { get; set; } = string.Empty;

    internal SelectionPlugin UseAutoComplete()
    {
        mAutoComplete = new AutoComplete<OptionItem>();

        return this;
    }

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter:
                return true;
            case ConsoleKey.UpArrow:
                Pagination.Prev();
                break;
            case ConsoleKey.DownArrow:
                Pagination.Next();
                break;
            default:
                if (mAutoComplete != null)
                {
                    mAutoComplete.InterceptInput(keyInfo);
                    Pagination.Home();
                }

                break;
        }

        return false;
    }

    List<IPromptComponent> IBasePlugin.Render()
    {
        List<OptionItem> options = Options;
        if (mAutoComplete != null)
        {
            int[] indexes = mAutoComplete.GetSuggestionIndexes(Options);
            options = indexes.Select(i => Options[i]).ToList();
            Pagination.Total = indexes.Length;
        }
        else
        {
            Pagination.Total = Options.Count;
        }

        List<IPromptComponent> components =
        [
            new TextComponent()
                .SetDecoration(EDecorationFlags.Bold)
                .SetText(Prompt)
                .UseLineBreak(),
        ];

        if (mAutoComplete != null)
        {
            components.Add(new TextComponent()
                .SetTextColor(EColorFlags.BrightBlack)
                .SetText(mAutoComplete.GetPrompt())
                .UseLineBreak()
            );
        }

        (int start, int end, int prevRest, int nextRest) = Pagination.GetRange();

        if (prevRest != 0)
        {
            for (int i = Pagination.Total - prevRest; i < Pagination.Total; ++i)
            {
                components.Add(options[i].ToRestTextComponent());
            }
        }

        for (int i = start; i < end; ++i)
        {
            components.Add(options[i].ToTextComponent(i == Pagination.Offset));
        }

        if (nextRest != 0)
        {
            for (int i = 0; i < nextRest; ++i)
            {
                components.Add(options[i].ToRestTextComponent());
            }
        }

        if (mAutoComplete != null)
        {
            components.Add(new CursorComponent()
                .AddCursors(
                    new { Direction = ECursorDirection.HorizontalAbsolute, Count = 0 },
                    new { Direction = ECursorDirection.Right, Count = mAutoComplete.GetPrompt().Length + mAutoComplete.Input.Length },
                    new { Direction = ECursorDirection.Up, Count = prevRest + (end - start) + nextRest + 1 }
                )
            );
        }

        return components;
    }
}
