namespace Sernager.Core.Tests.Fakes;


public sealed class FakeException : Exception
{
    public FakeException(string message) : base(message)
    {
    }

    public FakeException(string message, string arg1, string arg2) : base(message + arg1 + arg2)
    {
    }
}
