using Sernager.Core.Tests.Fixtures;
using Sernager.Core.Utils;

namespace Sernager.Core.Tests.Units.Utils;

public class RandomizerTest : BaseFixture
{
    private readonly string mAllChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    [Test]
    public void GenerateRandomString_WithFixedLength_ShouldReturnStringOfCorrectLength()
    {
        int length = 10;
        string result = Randomizer.GenerateRandomString(length);

        Assert.That(result.Length, Is.EqualTo(length));
    }

    [Test]
    public void GenerateRandomString_WithFixedLength_ShouldReturnStringWithOnlyAllowedChars()
    {
        int length = 10;
        string result = Randomizer.GenerateRandomString(length);

        Assert.That(result.All(mAllChars.Contains), Is.True);
    }

    [Test]
    public void GenerateRandomString_WithRangeLength_ShouldReturnStringOfCorrectLength()
    {
        int minLength = 5;
        int maxLength = 15;
        string result = Randomizer.GenerateRandomString(minLength, maxLength);

        Assert.That(result.Length, Is.GreaterThanOrEqualTo(minLength));
        Assert.That(result.Length, Is.LessThanOrEqualTo(maxLength));
    }

    [Test]
    public void GenerateRandomString_WithRangeLength_ShouldReturnStringWithOnlyAllowedChars()
    {
        int minLength = 5;
        int maxLength = 15;
        string result = Randomizer.GenerateRandomString(minLength, maxLength);

        Assert.That(result.All(mAllChars.Contains), Is.True);
    }

    [Test]
    public void GenerateRandomString_WithZeroLength_ShouldReturnEmptyString()
    {
        string result = Randomizer.GenerateRandomString(0);

        Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public void GenerateRandomString_WithNegativeLength_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => Randomizer.GenerateRandomString(-1));
    }

    [Test]
    public void GenerateRandomString_WithRangeLength_ShouldThrowException_WhenMinLengthIsGreaterThanMaxLength()
    {
        Assert.Throws<ArgumentException>(() => Randomizer.GenerateRandomString(10, 5));
    }

    [Test]
    public void GenerateRandomString_WithRangeLength_ShouldThrowException_WhenMinLengthIsNegative()
    {
        Assert.Throws<ArgumentException>(() => Randomizer.GenerateRandomString(-1, 10));
    }
}
