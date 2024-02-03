using Sernager.Core.Models;
using Sernager.Core.Tests.Fakes;

namespace Sernager.Core.Tests.Units.Models;

internal sealed class GroupItemModelFailureTests
{
    [DatapointSource]
    private static readonly object[] _ =
    [
        0,
        new int[0],
        'a',
        "string",
        new FakeModel(),
        new { Not = "Valid Model" },
    ];

    [Theory]
    public void ItemSetter_ShouldThrow_WhenInvalidTypeGiven(object testCase)
    {
        Assert.Throws<InvalidCastException>(() =>
        {
            GroupItemModel groupItemModel = new GroupItemModel()
            {
                Id = Guid.NewGuid(),
                Item = testCase
            };
        });
    }
}
