namespace Sernager.Terminal.Prompts.Plugins;

internal interface ITypePlugin<TResult> : IBasePlugin
    where TResult : notnull
{
}
