using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Cursors;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Prompts.Plugins;

internal abstract class ListBasePlugin<TOptionValue> : IBasePlugin
    where TOptionValue : notnull
{
    public readonly Pagination Pagination = new Pagination();
    public readonly List<OptionItem<TOptionValue>> Options = new List<OptionItem<TOptionValue>>();
    private protected AutoComplete<OptionItem<TOptionValue>>? mAutoComplete = null;
    public string Prompt { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo, out object result)
    {
        result = null!;

        return false;
    }

    List<IPromptComponent> IBasePlugin.Render()
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

        if (!string.IsNullOrWhiteSpace(Description))
        {
            components.Add(new TextComponent()
                .SetTextColor(EColorFlags.BrightBlack)
                .SetText(Description)
                .UseLineBreak()
            );
        }

        if (mAutoComplete != null)
        {
            components.Add(new TextComponent()
                .SetTextColor(EColorFlags.Magenta)
                .SetText(mAutoComplete.GetPrompt())
            );

            components.Add(new TextComponent()
                .SetText(mAutoComplete.Input)
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
            if (end == 0)
            {
                components.Add(new TextComponent()
                    .SetTextColor(EColorFlags.Red)
                    .SetText(mAutoComplete.GetNoResult())
                    .UseLineBreak()
                );
            }

            components.Add(new CursorComponent()
                .AddCursors(
                    new { Direction = ECursorDirection.HorizontalAbsolute, Count = 0 },
                    new { Direction = ECursorDirection.Right, Count = mAutoComplete.GetPrompt().Length + mAutoComplete.CursorPosition },
                    new { Direction = ECursorDirection.Up, Count = prevRest + (end - start) + nextRest + 1 + (end == 0 ? 1 : 0) }
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
