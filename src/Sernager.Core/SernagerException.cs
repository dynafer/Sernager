using System.Diagnostics.CodeAnalysis;

namespace Sernager.Core;

[ExcludeFromCodeCoverage]
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
