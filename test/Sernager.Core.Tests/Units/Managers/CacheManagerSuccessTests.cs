using Sernager.Core.Managers;
using Sernager.Core.Tests.Fakes;

namespace Sernager.Core.Tests.Units.Managers;

public class CacheManagerSuccessTests
{
    [TearDown]
    public void ClearCache()
    {
        CacheManager.Clear();
    }

    [Test]
    public void TryGet_And_Set_ShouldReturnTypedValue()
    {
        CacheManager.Set("success_key", "string type");

        string? stringValue;
        bool result = CacheManager.TryGet("success_key", out stringValue);

        Assert.That(result, Is.True);
        Assert.That(stringValue, Is.Not.Null);
        Assert.That(stringValue, Is.EqualTo("string type"));

        FakeModel fakeModel = new FakeModel();

        CacheManager.Set("success_key", fakeModel);

        FakeModel? fakeModelValue;
        result = CacheManager.TryGet("success_key", out fakeModelValue);

        Assert.That(result, Is.True);
        Assert.That(fakeModelValue, Is.Not.Null);
        Assert.That(fakeModelValue, Is.EqualTo(fakeModel));
    }

    [Test]
    public void Remove_ShouldRemoveCachedValue()
    {
        CacheManager.Set("removal_key", "string type");

        string? stringValue;
        bool result = CacheManager.TryGet("removal_key", out stringValue);

        Assert.That(result, Is.True);
        Assert.That(stringValue, Is.Not.Null);
        Assert.That(stringValue, Is.EqualTo("string type"));

        CacheManager.Remove("removal_key");

        result = CacheManager.TryGet("removal_key", out stringValue);

        Assert.That(result, Is.False);
        Assert.That(stringValue, Is.Null);
    }
}
