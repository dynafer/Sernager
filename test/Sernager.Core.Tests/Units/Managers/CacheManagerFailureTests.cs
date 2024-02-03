using Sernager.Core.Managers;
using Sernager.Core.Tests.Fakes;

namespace Sernager.Core.Tests.Units.Managers;

internal sealed class CacheManagerFailureTests : FailureFixture
{
    [TearDown]
    public void ClearCache()
    {
        CacheManager.Clear();
    }

    [Test]
    public void TryGet_ShouldReturnFalseAndOutNull_WhenPassedNotExistingKey()
    {
        object? value;
        bool result = CacheManager.TryGet("not-existing-key", out value);

        Assert.That(result, Is.False);
        Assert.That(value, Is.Null);
    }

    [Test]
    public void TryGet_ShouldReturnFalseAndOutNull_WhenPassedUncastableType()
    {
        CacheManager.Set("failure_case", "string type");

        FakeModel? fakeModel = null;

        TestNoneLevel(() => CacheManager.TryGet("failure_case", out fakeModel), Is.False);
        Assert.That(fakeModel, Is.Null);
        TestExceptionLevel<InvalidCastException>(() => CacheManager.TryGet("failure_case", out fakeModel));
        Assert.That(fakeModel, Is.Null);

        CacheManager.Set("failure_case", new FakeModel());

        string? stringValue = null;

        TestNoneLevel(() => CacheManager.TryGet("failure_case", out stringValue), Is.False);
        Assert.That(stringValue, Is.Null);
        TestExceptionLevel<InvalidCastException>(() => CacheManager.TryGet("failure_case", out stringValue));
        Assert.That(stringValue, Is.Null);
    }
}
