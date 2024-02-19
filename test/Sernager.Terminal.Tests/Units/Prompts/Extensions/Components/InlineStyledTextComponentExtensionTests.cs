using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Extensions.Components;

namespace Sernager.Terminal.Tests.Units.Prompts.Extensions.Components;

internal sealed class InlineStyledTextComponentExtensionTests
{
    [Test]
    public void SetText_ShouldSetText()
    {
        InlineStyledTextComponent component = new InlineStyledTextComponent();
        component.SetText("Hello, World!");

        Assert.That(component.Text, Is.EqualTo("Hello, World!"));
    }
}
