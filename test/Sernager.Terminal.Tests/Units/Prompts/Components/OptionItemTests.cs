using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Tests.Units.Prompts.Components;

internal sealed class OptionItemTests
{
    [DatapointSource]
    private static readonly EOptionTypeFlags[] OPTION_TYPES = Enum.GetValues<EOptionTypeFlags>();
    [DatapointSource]
    private static readonly bool[] BOOLS = { true, false };

    [Theory]
    public void Constructor_ShouldInitialise(EOptionTypeFlags type, bool bUseResourcePack)
    {
        Assume.That(type, Is.AnyOf(OPTION_TYPES));

        string name = "Test";
        int value = 1;

        OptionItem<int> optionItem = new OptionItem<int>(type, name, value, bUseResourcePack);

        Assert.That(optionItem.Name, Is.EqualTo(name));
        Assert.That(optionItem.Value, Is.EqualTo(value));
        Assert.That(optionItem.IsSelected, Is.EqualTo(bUseResourcePack));
        Assert.That(PrivateUtil.GetFieldValue<bool>(optionItem, "mbUseResourcePack"), Is.EqualTo(bUseResourcePack));
    }

    [Theory]
    public void ToggleSelection_ShouldToggleIsSelected(EOptionTypeFlags type)
    {
        Assume.That(type, Is.AnyOf(OPTION_TYPES));

        string name = "Test";
        int value = 1;

        OptionItem<int> optionItem = new OptionItem<int>(type, name, value, true);

        optionItem.ToggleSelection();

        Assert.That(optionItem.IsSelected, Is.True);

        optionItem.ToggleSelection();

        Assert.That(optionItem.IsSelected, Is.False);
    }

    [Theory]
    public void ToTextComponent_ShouldReturnTextComponent(EOptionTypeFlags type, bool bUseResourcePack)
    {
        Assume.That(type, Is.AnyOf(OPTION_TYPES));

        string name = "Test";
        int value = 1;

        OptionItem<int> optionItem = new OptionItem<int>(type, name, value, bUseResourcePack);

        // TODO: Add test for ToTextComponent after testing Plugins
    }

    [Theory]
    public void ToRestTextComponent_ShouldReturnTextComponent(EOptionTypeFlags type, bool bUseResourcePack)
    {
        Assume.That(type, Is.AnyOf(OPTION_TYPES));

        string name = "Test";
        int value = 1;

        OptionItem<int> optionItem = new OptionItem<int>(type, name, value, bUseResourcePack);

        // TODO: Add test for ToRestTextComponent after testing Plugins
    }
}
