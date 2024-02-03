using Sernager.Core.Configs;
using Sernager.Core.Extensions;
using Sernager.Core.Options;
using Sernager.Core.Tests.Fixtures;

namespace Sernager.Core.Tests.Units.Extensions;

internal sealed class EnvironmentManagerExtensionSuccessTests : EnvironmentManagerFixture
{
    [Theory]
    public void UseMode_ShouldChangeMode(EAddDataOption additionMode)
    {
        Assume.That(additionMode, Is.AnyOf(ADDITION_MODES));

        mManager.UseMode(additionMode);

        Assert.That(mManager.AdditionMode, Is.EqualTo(additionMode));
    }

    [Theory]
    public void ChangeGroupName_ShouldChangeName()
    {
        string name = "TEST_GROUP";

        mManager.ChangeGroupName(name);

        Assert.That(Configurator.Config.EnvironmentGroups.ContainsKey(name), Is.True);
        Assert.That(mManager.Group.Name, Is.EqualTo(name));
    }
}
