using Sernager.Core.Managers;
using Sernager.Core.Options;
using Sernager.Core.Tests.Fakes;

namespace Sernager.Core.Tests.Units.Managers;

public class ExceptionManagerSuccessTests
{
    [DatapointSource]
    private static readonly EErrorLevel[] TEST_ERROR_LEVELS =
    [
        EErrorLevel.None,
        EErrorLevel.Exception
    ];

    [Theory]
    public void Throw_ShouldThrowExceptionOrNot(EErrorLevel level)
    {
        Assume.That(level, Is.AnyOf(TEST_ERROR_LEVELS));

        ExceptionManager.ErrorLevel = level;

        if (level != EErrorLevel.None)
        {
            Assert.Throws<FakeException>(() => ExceptionManager.Throw<FakeException>("message"));
        }
        else
        {
            ExceptionManager.Throw<FakeException>("message");

            Assert.Pass();
        }
    }

    [Theory]
    public void ThrowFaile_ShouldThrowExceptionOrNot(EErrorLevel level)
    {
        Assume.That(level, Is.AnyOf(TEST_ERROR_LEVELS));

        ExceptionManager.ErrorLevel = level;

        if (level != EErrorLevel.None)
        {
            Assert.Throws<FakeException>(() => ExceptionManager.ThrowFail<FakeException>("message"));
        }
        else
        {
            ExceptionManager.ThrowFail<FakeException>("message");

            Assert.Pass();
        }
    }
}
