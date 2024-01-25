using Sernager.Core.Managers;
using Sernager.Core.Options;
using System.Diagnostics;

namespace Sernager.Unit.Fixtures;

public abstract class FailureFixture
{
    [DatapointSource]
    public static readonly EErrorLevel[] TESTABLE_LEVELS =
    [
        EErrorLevel.None,
        EErrorLevel.Exception
    ];

    [StackTraceHidden]
    public void TestNoneLevel<TActual>(Func<TActual> actual, IResolveConstraint expression)
    {
        ExceptionManager.ErrorLevel = EErrorLevel.None;

        Assert.That(actual(), expression);
    }

    [StackTraceHidden]
    public void TestNoneLevel(TestDelegate testDelegate)
    {
        ExceptionManager.ErrorLevel = EErrorLevel.None;

        Assert.DoesNotThrow(testDelegate);
    }

    [StackTraceHidden]
    public void TestExceptionLevel<T>(TestDelegate testDelegate)
        where T : Exception
    {
        ExceptionManager.ErrorLevel = EErrorLevel.Exception;

        Assert.Throws<T>(testDelegate);
    }
}
