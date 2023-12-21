using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Factories.Plugins;

internal sealed class SelectionPlugin<TOptionValue> : ListBasePlugin<TOptionValue>, ITypePlugin<TOptionValue>
    where TOptionValue : notnull
{
    internal SelectionPlugin<TOptionValue> UseAutoComplete()
    {
        mAutoComplete = new AutoComplete<OptionItem<TOptionValue>>();

        return this;
    }
}
