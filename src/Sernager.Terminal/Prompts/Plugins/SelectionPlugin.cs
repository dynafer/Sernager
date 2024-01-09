using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Cursors;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Prompts.Plugins;

internal sealed class SelectionPlugin<TOptionValue> : ListBasePlugin<TOptionValue>, ITypePlugin<TOptionValue>
    where TOptionValue : notnull
{
    private string mResult = string.Empty;

    internal SelectionPlugin<TOptionValue> UseAutoComplete()
    {
        mAutoComplete = new AutoComplete<OptionItem<TOptionValue>>();

        return this;
    }

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo, out object result)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter:
                (List<OptionItem<TOptionValue>> options, int _) = getOptions();
                result = options[Pagination.Offset].Value;
                mResult = options[Pagination.Offset].Name;

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
