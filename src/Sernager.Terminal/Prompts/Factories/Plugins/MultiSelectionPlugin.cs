using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Factories.Plugins;

internal sealed class MultiSelectionPlugin<TOptionValue> : ListBasePlugin<TOptionValue>, IEnumerableResultBasePlugin<TOptionValue>
    where TOptionValue : notnull
{
    internal MultiSelectionPlugin<TOptionValue> UseAutoComplete()
    {
        mAutoComplete = new AutoComplete<OptionItem<TOptionValue>>();

        return this;
    }

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo, out object result)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter:
            {
                (List<OptionItem<TOptionValue>> options, int _) = getOptions();
                result = options.Select(x => x.Value);

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
}
