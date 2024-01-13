using Sernager.Core.Utils;
using Sernager.Unit.Tests.Fixtures;

namespace Sernager.Core.Tests.Units.Utils;

public class RandomizerFailureTests : FailureFixture
{
    [Test]
    public void GenerateRandomString_ShouldThrow_WhenPassedNegativeLength()
    {
        TestNoneLevel(() => Randomizer.GenerateRandomString(-1), Is.Empty);
        TestExceptionLevel<ArgumentException>(() => Randomizer.GenerateRandomString(-1));
    }

    [Test]
    public void GenerateRandomString_ShouldThrow_WhenPassedNegativeMinLength()
    {
        TestNoneLevel(() => Randomizer.GenerateRandomString(-1, 0), Is.Empty);
        TestExceptionLevel<ArgumentException>(() => Randomizer.GenerateRandomString(-1, 0));
    }

    [Test]
    public void GenerateRandomString_ShouldThrow_WhenPassedNegativeMaxLength()
    {
        TestNoneLevel(() => Randomizer.GenerateRandomString(0, -1), Is.Empty);
        TestExceptionLevel<ArgumentException>(() => Randomizer.GenerateRandomString(0, -1));
    }

    [Test]
    public void GenerateRandomString_ShouldThrow_WhenPassedMinLengthGreaterThanMaxLength()
    {
        TestNoneLevel(() => Randomizer.GenerateRandomString(1, 0), Is.Empty);
        TestExceptionLevel<ArgumentException>(() => Randomizer.GenerateRandomString(1, 0));
    }

    [Test]
    public void GenerateRandomString_ShouldThrow_WhenPassedNegativeMinLengthAndMaxLength()
    {
        TestNoneLevel(() => Randomizer.GenerateRandomString(-1, -1), Is.Empty);
        TestExceptionLevel<ArgumentException>(() => Randomizer.GenerateRandomString(-1, -1));
    }
}
