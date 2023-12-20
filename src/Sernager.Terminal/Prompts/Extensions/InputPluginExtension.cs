using Sernager.Terminal.Prompts.Factories.Plugins;

namespace Sernager.Terminal.Prompts.Extensions;

internal static class InputPluginExtension
{
    internal static InputPlugin AddHint(this InputPlugin plugin, string hint)
    {
        if (plugin.Hints == null)
        {
            throw new InvalidOperationException("You must call UseAutoComplete() before adding hints.");
        }

        plugin.Hints.Add(hint);

        return plugin;
    }

    internal static InputPlugin AddHints(this InputPlugin plugin, params string[] hints)
    {
        if (plugin.Hints == null)
        {
            throw new InvalidOperationException("You must call UseAutoComplete() before adding hints.");
        }

        plugin.Hints.AddRange(hints);

        return plugin;
    }

    internal static InputPlugin ShowHints(this InputPlugin plugin)
    {
        plugin.ShouldShowHints = true;

        return plugin;
    }
}
