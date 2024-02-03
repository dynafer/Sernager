using Sernager.Core.Extensions;
using Sernager.Core.Tests.Fakes;

namespace Sernager.Core.Tests.Units.Extensions;

internal sealed class EnumExtensionTests
{
    [DatapointSource]
    private static readonly EFake[] FAKE_TYPES = Enum.GetValues<EFake>();

    [Theory]
    public void GetDescription_ShouldReturnDescription(EFake fake)
    {
        Assume.That(fake, Is.AnyOf(FAKE_TYPES));

        string description = fake.GetDescription();

        string expectation = fake switch
        {
            EFake.Fake1 => "Fake 1",
            EFake.Fake2 => "Fake 2",
            EFake.Fake3 => string.Empty,
            _ => string.Empty
        };

        Assert.That(description, Is.EqualTo(expectation));
    }

    [Test]
    public void GetDescription_ShouldReturnEmptyString()
    {
        string description = ((EFake)int.MaxValue).GetDescription();

        Assert.That(description, Is.EqualTo(string.Empty));
    }
}
