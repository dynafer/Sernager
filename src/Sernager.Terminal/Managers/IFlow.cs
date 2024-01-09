namespace Sernager.Terminal.Managers;

internal interface IFlow
{
    internal void Prompt();
    internal bool TryJump(string command, bool bHasNext);
}
