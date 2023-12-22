using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Prompts.Plugins;

internal sealed class SelectionPlugin<TOptionValue> : ListBasePlugin<TOptionValue>, ITypePlugin<TOptionValue>
    where TOptionValue : notnull
{
    internal SelectionPlugin<TOptionValue> UseAutoComplete()
    {
        mAutoComplete = new AutoComplete<OptionItem<TOptionValue>>();

        return this;
    }
}
