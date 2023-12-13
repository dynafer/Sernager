namespace Sernager.Core;

public class SernagerException : Exception
{
    public SernagerException()
    {
    }

    public SernagerException(string? message)
        : base(message)
    {
    }
}
