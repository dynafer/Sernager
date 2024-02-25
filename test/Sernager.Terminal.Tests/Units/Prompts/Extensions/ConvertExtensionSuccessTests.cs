using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Extensions;

namespace Sernager.Terminal.Tests.Units.Prompts.Extensions;

internal sealed class ConvertExtensionSuccessTests
{
    [DatapointSource]
    private static readonly EOptionTypeFlags[] OPTION_TYPE_FLAGS = Enum.GetValues<EOptionTypeFlags>();

    [Test]
    public void ToSuggestItem_ShouldReturnString_WhenItemIsString()
    {
        string item = "test";
        string result = item.ToSuggestItem();

        Assert.That(result, Is.EqualTo(item));
    }

    [Theory]
    public void ToSuggestItem_ShouldReturnName_WhenItemIsOptionItem(EOptionTypeFlags type)
    {
        Assume.That(type, Is.AnyOf(OPTION_TYPE_FLAGS));

        OptionItem<object> itemWithObjectType = new OptionItem<object>(type, "test_name", new object(), false);
        OptionItem<string> itemWithStringType = new OptionItem<string>(type, "test_name", "test data", false);

        string objectTypeResult = itemWithObjectType.ToSuggestItem();
        string stringTypeResult = itemWithStringType.ToSuggestItem();

        Assert.That(objectTypeResult, Is.EqualTo("test_name"));
        Assert.That(stringTypeResult, Is.EqualTo("test_name"));
    }

    [Test]
    public void ToResult_ShouldReturnResult_WhenObjectCanBeCasted()
    {
        object stringObj = "test";
        object intObj = 1;
        object boolObj = true;

        string stringCasted = stringObj.ToResult<string>();
        int intCasted = intObj.ToResult<int>();
        bool boolCasted = boolObj.ToResult<bool>();

        Assert.That(stringCasted, Is.EqualTo("test"));
        Assert.That(intCasted, Is.EqualTo(1));
        Assert.That(boolCasted, Is.EqualTo(true));
    }

    [Test]
    public void ToOptionType_ShouldReturnEOptionTypeFlags_WhenPluginIsSupported()
    {
        // TODO: Add test for ToRestTextComponent after testing Plugins
    }
}
