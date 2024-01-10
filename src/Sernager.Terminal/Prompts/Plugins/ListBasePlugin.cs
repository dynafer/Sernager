using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Cursors;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Prompts.Plugins;

internal abstract class ListBasePlugin<TOptionValue> : IBasePlugin
    where TOptionValue : notnull
{
    private protected AutoComplete<OptionItem<TOptionValue>>? mAutoComplete = null;
    internal Pagination Pagination { get; private init; } = new Pagination();
    internal List<OptionItem<TOptionValue>> Options { get; private init; } = new List<OptionItem<TOptionValue>>();
    public string Prompt { get; set; } = string.Empty;
    public List<string> Description { get; init; } = new List<string>();
    public bool ShouldShowCursor => mAutoComplete != null;

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo, out object result)
    {
        result = null!;

        return false;
    }

    public virtual List<IPromptComponent> Render()
    {
        (List<OptionItem<TOptionValue>> options, int total) = getOptions();
        Pagination.Total = total;

        List<IPromptComponent> components =
        [
            new TextComponent()
                .SetDecoration(EDecorationFlags.Bold)
                .SetText(Prompt)
                .UseLineBreak(),
        ];

        if (Description.Count > 0)
        {
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

        if (mAutoComplete != null)
        {
            components.Add(new TextComponent()
                .SetTextColor(EColorFlags.Magenta)
                .SetText(mAutoComplete.Prompt)
            );

            components.Add(new TextComponent()
                .SetText(mAutoComplete.Input)
                .UseLineBreak()
            );
        }

        (int start, int end, int prev, int next) = Pagination.GetRange();

        if (prev != 0)
        {
            for (int i = Pagination.Total - prev; i < Pagination.Total; ++i)
            {
                components.Add(options[i].ToRestTextComponent());
            }
        }

        for (int i = start; i < end; ++i)
        {
            TextComponent option = options[i].ToTextComponent(i == Pagination.Offset);
            if (next == 0 && i == end - 1)
            {
                option.IsLineBreak = false;
            }

            components.Add(option);
        }

        if (next != 0)
        {
            for (int i = 0; i < next; ++i)
            {
                TextComponent option = options[i].ToRestTextComponent();
                if (i == next - 1)
                {
                    option.IsLineBreak = false;
                }

                components.Add(option);
            }
        }

        if (mAutoComplete != null)
        {
            bool bNoResult = mAutoComplete.Input.Length != 0 && end == 0;

            if (bNoResult)
            {
                components.Add(new TextComponent()
                    .SetTextColor(EColorFlags.Red)
                    .SetText(mAutoComplete.NoResultText)
                );
            }

            components.Add(new CursorComponent()
                .AddCursors(
                    new { Direction = ECursorDirection.HorizontalAbsolute, Count = 0 },
                    new { Direction = ECursorDirection.Right, Count = mAutoComplete.Prompt.Length + mAutoComplete.CursorPosition },
                    new { Direction = ECursorDirection.Up, Count = prev + (end - start) + next + (bNoResult ? 1 : 0) }
                )
            );
        }

        return components;
    }

    List<IPromptComponent> IBasePlugin.RenderLast()
    {
        return new List<IPromptComponent>();
    }

    private protected (List<OptionItem<TOptionValue>>, int) getOptions()
    {
        if (mAutoComplete != null)
        {
            int[] indexes = mAutoComplete.GetSuggestionIndexes(Options);
            return (indexes.Select(i => Options[i]).ToList(), indexes.Length);
        }

        return (Options, Options.Count);
    }
}
