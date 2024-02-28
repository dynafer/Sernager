using Sernager.Resources;
using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Cursors;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;
using Sernager.Terminal.Prompts.Helpers;
using Sernager.Terminal.Prompts.Plugins.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace Sernager.Terminal.Prompts.Plugins;

internal abstract class ListBasePlugin<TOptionValue> : IBasePlugin
    where TOptionValue : notnull
{
    private protected AutoComplete<OptionItem<TOptionValue>>? mAutoComplete = null;
    internal Pagination Pagination { get; private init; } = new Pagination();
    internal List<OptionItem<TOptionValue>> Options { get; private init; } = new List<OptionItem<TOptionValue>>();
    public IResourcePack? ResourcePack { get; set; } = null;
    public string Prompt { get; set; } = string.Empty;
    public List<string> Description { get; init; } = new List<string>();
    public bool ShouldShowCursor => mAutoComplete != null;

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo, [NotNullWhen(true)] out object? result)
    {
        result = null;

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
                .SetText(PluginResourceHelper.GetString(this, Prompt))
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
                            .SetText(PluginResourceHelper.GetString(this, description))
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

        PaginationRange range = Pagination.GetRange();

        if (range.Prev != 0)
        {
            for (int i = Pagination.Total - range.Prev; i < Pagination.Total; ++i)
            {
                components.Add(options[i].ToRestTextComponent(this));
            }
        }

        for (int i = range.Start; i < range.End; ++i)
        {
            TextComponent option = options[i].ToTextComponent(this, i == Pagination.Offset);
            if (range.Next == 0 && i == range.End - 1)
            {
                option.IsLineBreak = false;
            }

            components.Add(option);
        }

        if (range.Next != 0)
        {
            for (int i = 0; i < range.Next; ++i)
            {
                TextComponent option = options[i].ToRestTextComponent(this);
                if (i == range.Next - 1)
                {
                    option.IsLineBreak = false;
                }

                components.Add(option);
            }
        }

        if (mAutoComplete != null)
        {
            bool bNoResult = mAutoComplete.Input.Length != 0 && range.End == 0;

            if (bNoResult)
            {
                components.Add(new TextComponent()
                    .SetTextColor(EColorFlags.Red)
                    .SetText(mAutoComplete.NoResultText)
                );
            }

            components.Add(new CursorComponent()
                .AddCursors(
                    new PromptCursor(ECursorDirection.HorizontalAbsolute, 0),
                    new PromptCursor(ECursorDirection.Right, mAutoComplete.Prompt.Length + mAutoComplete.CursorPosition),
                    new PromptCursor(ECursorDirection.Up, range.Prev + (range.End - range.Start) + range.Next + (bNoResult ? 1 : 0))
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
