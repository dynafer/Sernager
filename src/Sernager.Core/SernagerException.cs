namespace Sernager.Core;

public sealed class SernagerException : Exception
{
    public SernagerException()
    {
    }

    public SernagerException(string? message)
        : base(message)
    {
    }
}
