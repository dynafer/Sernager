using Sernager.Resources;
using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Cursors;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;
using Sernager.Terminal.Prompts.Helpers;
using Sernager.Terminal.Prompts.Plugins.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace Sernager.Terminal.Prompts.Plugins;

internal sealed class MultiSelectionPlugin<TOptionValue> : ListBasePlugin<TOptionValue>, IEnumerableResultBasePlugin<TOptionValue>
    where TOptionValue : notnull
{
    private string[] mResult = Array.Empty<string>();
    private List<string> mInitialSelections = new List<string>();
    private bool mbRendered = false;

    internal MultiSelectionPlugin<TOptionValue> UseAutoComplete()
    {
        mAutoComplete = new AutoComplete<OptionItem<TOptionValue>>();

        return this;
    }

    internal MultiSelectionPlugin<TOptionValue> SetInitialSelections(params string[] selections)
    {
        foreach (string selection in selections)
        {
            if (mInitialSelections.Contains(selection))
            {
                continue;
            }

            mInitialSelections.Add(selection);
        }

        return this;
    }

    public override List<IPromptComponent> Render()
    {
        if (!mbRendered)
        {
            mbRendered = true;

            Options.ForEach(option =>
            {
                if (mInitialSelections.Contains(option.Name))
                {
                    option.ToggleSelection();
                }
            });

            mInitialSelections.Clear();
            mInitialSelections = null!;
        }

        List<IPromptComponent> components = base.Render();

        return components;
    }

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo, [NotNullWhen(true)] out object? result)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter:
            {
                (List<OptionItem<TOptionValue>> options, _) = getOptions();
                result = options.Where(x => x.IsSelected).Select(x => x.Value);
                mResult = options.Where(x => x.IsSelected).Select(x => x.Name).ToArray();

                return true;
            }
            case ConsoleKey.UpArrow:
                Pagination.Prev();
                break;
            case ConsoleKey.DownArrow:
                Pagination.Next();
                break;
            case ConsoleKey.Spacebar:
            {
                (List<OptionItem<TOptionValue>> options, _) = getOptions();
                options[Pagination.Offset].ToggleSelection();
                break;
            }
            default:
                if (mAutoComplete != null)
                {
                    mAutoComplete.InterceptInput(keyInfo);
                    Pagination.Home();
                }

                break;
        }

        result = null;

        return false;
    }

    List<IPromptComponent> IBasePlugin.RenderLast()
    {
        List<IPromptComponent> components =
        [
            new TextComponent()
                .SetDecoration(EDecorationFlags.Bold)
                .SetText(PluginResourceHelper.GetString(this, Prompt)),
            new CursorComponent()
                .AddCursor(ECursorDirection.Right, 1),
        ];

        if (mResult.Length > 0)
        {
            components.Add(
                new TextComponent()
                    .SetDecoration(EDecorationFlags.Bold)
                    .SetTextColor(EColorFlags.Green)
                    .SetText(string.Join(", ", mResult))
                    .UseLineBreak()
            );
        }
        else
        {
            components.Add(
                new TextComponent()
                    .SetDecoration(EDecorationFlags.Bold)
                    .SetTextColor(EColorFlags.Red)
                    .SetText(ResourceRetriever.Shared.GetString("Cancel"))
                    .UseLineBreak()
            );
        }

        return components;
    }
}
