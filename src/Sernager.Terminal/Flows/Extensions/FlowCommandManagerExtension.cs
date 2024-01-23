using Sernager.Core.Managers;

namespace Sernager.Terminal.Flows.Extensions;

internal static class FlowCommandManagerExtension
{
    internal static string CreateCommandGroupBreadcrumb(this ICommandManager manager, string delimiter)
    {
        return string.Join(delimiter, manager.GetBreadcrumb());
    }

    internal static string CreateCommandBreadcrumb(this ICommandManager manager, string delimiter, string commandName)
    {
        return $"{manager.CreateCommandGroupBreadcrumb(delimiter)}{delimiter}{commandName}";
    }

    internal static string CreateCommandGroupFlowName(this ICommandManager manager, string prefix)
    {
        return $"{manager.CreateCommandGroupBreadcrumb(".")}.{prefix}";
    }

    internal static string CreateCommandGroupFlowName(this ICommandManager manager, string prefix, string flowName)
    {
        return $"{manager.CreateCommandGroupFlowName(prefix)}.{flowName}";
    }

    internal static string CreateCommandFlowName(this ICommandManager manager, string prefix, string commandName)
    {
        return $"{manager.CreateCommandBreadcrumb(".", commandName)}.{prefix}";
    }

    internal static string CreateCommandFlowName(this ICommandManager manager, string prefix, string commandName, string flowName)
    {
        return $"{manager.CreateCommandFlowName(prefix, commandName)}.{flowName}";
    }
}
