using Sernager.Core.Utils;

namespace Sernager.Core.Tests.Units.Utils;

internal sealed class RandomizerSuccessTests
{
    [Test]
    public void GenerateRandomString_ShouldReturnEmptyString_WhenPassedZeroLength()
    {
        string randomString = Randomizer.GenerateRandomString(0);

        Assert.That(randomString, Is.Not.Null);
        Assert.That(randomString, Is.Empty);
    }

    [Test]
    public void GenerateRandomString_ShouldReturnRandomString_WhenPassedValidLength()
    {
        string randomString = Randomizer.GenerateRandomString(10);

        Assert.That(randomString, Is.Not.Null);
        Assert.That(randomString.Length, Is.EqualTo(10));
    }

    [Test]
    public void GenerateRandomString_ShouldReturnRandomString_WhenPassedValidMinAndMaxLengths()
    {
        string randomString = Randomizer.GenerateRandomString(10, 20);

        Assert.That(randomString, Is.Not.Null);
        Assert.That(randomString.Length, Is.GreaterThanOrEqualTo(10));
        Assert.That(randomString.Length, Is.LessThanOrEqualTo(20));
    }

    [Test]
    public void GenerateRandomString_ShouldReturnRandomString_WhenPassedSameMinAndMaxLengths()
    {
        string randomString = Randomizer.GenerateRandomString(10, 10);

        Assert.That(randomString, Is.Not.Null);
        Assert.That(randomString.Length, Is.EqualTo(10));
    }
}
