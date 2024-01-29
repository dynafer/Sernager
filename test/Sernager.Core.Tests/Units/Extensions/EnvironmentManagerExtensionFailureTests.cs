using Sernager.Core.Extensions;
using Sernager.Core.Options;
using Sernager.Core.Tests.Fixtures;

namespace Sernager.Core.Tests.Units.Extensions;

public class EnvironmentManagerExtensionFailureTests : EnvironmentManagerFixture
{
    [Theory]
    public void UseMode_ShouldThrow_WhenGroupAlreadyRemoved(EAddDataOption additionMode)
    {
        Assume.That(additionMode, Is.AnyOf(ADDITION_MODES));

        mManager.AdditionMode = additionMode;

        mManager.RemoveGroup();

        TestNoneLevel(() => mManager.UseMode(additionMode), Is.EqualTo(mManager));
        TestExceptionLevel<SernagerException>(() => mManager.UseMode(additionMode));
    }

    [Theory]
    public void ChangeGroupName_ShouldThrow_WhenGroupAlreadyRemoved()
    {
        string name = "TEST_GROUP";

        mManager.RemoveGroup();

        TestNoneLevel(() => mManager.ChangeGroupName(name), Is.False);
        TestExceptionLevel<SernagerException>(() => mManager.ChangeGroupName(name));
    }
}
