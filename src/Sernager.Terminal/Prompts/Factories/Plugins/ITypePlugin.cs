namespace Sernager.Terminal.Prompts.Factories.Plugins;

internal interface ITypePlugin<TResult> : IBasePlugin
    where TResult : notnull
{
}
