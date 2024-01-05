using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Cursors;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Prompts.Plugins;

internal sealed class MultiSelectionPlugin<TOptionValue> : ListBasePlugin<TOptionValue>, IEnumerableResultBasePlugin<TOptionValue>
    where TOptionValue : notnull
{
    private List<string> mResult = new List<string>();

    internal MultiSelectionPlugin<TOptionValue> UseAutoComplete()
    {
        mAutoComplete = new AutoComplete<OptionItem<TOptionValue>>();

        return this;
    }

    internal MultiSelectionPlugin<TOptionValue> SetInitialSelections(params string[] selections)
    {
        foreach (string selection in selections)
        {
            OptionItem<TOptionValue>? option = Options.FirstOrDefault(x => x.Name == selection);

            if (option != null)
            {
                option.ToggleSelection();
            }
        }

        return this;
    }

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo, out object result)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter:
            {
                (List<OptionItem<TOptionValue>> options, int _) = getOptions();
                result = options.Where(x => x.IsSelected).Select(x => x.Value);
                mResult = options.Where(x => x.IsSelected).Select(x => x.Name).ToList();

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
                (List<OptionItem<TOptionValue>> options, int _) = getOptions();
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

        result = null!;

        return false;
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
                .SetText(string.Join(", ", mResult))
                .UseLineBreak(),
        ];

        return components;
    }
}
