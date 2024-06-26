using Sernager.Core.Managers;
using Sernager.Core.Options;
using Sernager.Core.Tests.Fakes;

namespace Sernager.Core.Tests.Units.Managers;

internal sealed class ExceptionManagerFailureTests : FailureFixture
{
    [DatapointSource]
    private static readonly EErrorLevel[] TEST_ERROR_LEVELS = Enum.GetValues<EErrorLevel>();

    [Theory]
    public void Throw_ShouldThrow_WhenPassedInvalidArguments(EErrorLevel level)
    {
        Assume.That(level, Is.AnyOf(TEST_ERROR_LEVELS));

        ExceptionManager.ErrorLevel = level;

        if (level == EErrorLevel.None)
        {
            Assert.Pass();

            return;
        }

        Assert.Throws<NullReferenceException>(() => ExceptionManager.Throw<FakeException>("message", 1));
        Assert.Throws<NullReferenceException>(() => ExceptionManager.Throw<FakeException>("message", 1, 2));
        Assert.Throws<NullReferenceException>(() => ExceptionManager.Throw<FakeException>("message", string.Empty, 1));
        Assert.Throws<NullReferenceException>(() => ExceptionManager.Throw<FakeException>("message", 1, string.Empty));
    }

    [Theory]
    public void ThrowFail_ShouldThrow_WhenPassedInvalidArguments(EErrorLevel level)
    {
        Assume.That(level, Is.AnyOf(TEST_ERROR_LEVELS));

        ExceptionManager.ErrorLevel = level;

        if (level == EErrorLevel.None)
        {
            Assert.Pass();

            return;
        }

        Assert.Throws<NullReferenceException>(() => ExceptionManager.ThrowFail<FakeException>("message", 1));
        Assert.Throws<NullReferenceException>(() => ExceptionManager.ThrowFail<FakeException>("message", 1, 2));
        Assert.Throws<NullReferenceException>(() => ExceptionManager.ThrowFail<FakeException>("message", string.Empty, 1));
        Assert.Throws<NullReferenceException>(() => ExceptionManager.ThrowFail<FakeException>("message", 1, string.Empty));
    }
}
