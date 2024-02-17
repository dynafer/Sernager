using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Tests.Units.Prompts.Components;

internal sealed class InlineStyledTextComponentTests
{
    [Test]
    public void UsePlainTextOnly_ShouldUsePlainTextOnlyOption()
    {
        InlineStyledTextComponent component = new InlineStyledTextComponent();

        Assert.That(PrivateUtil.GetFieldValue<bool>(component, "mbPlainTextOnly"), Is.False);

        component.UsePlainTextOnly();

        Assert.That(PrivateUtil.GetFieldValue<bool>(component, "mbPlainTextOnly"), Is.True);
    }

    [Test]
    public void Render_ShouldReturnRenderedText()
    {
        string[][] caseAndExpectations = CaseUtil.ReadCSV("Prompts.InlineStyledTexts");

        for (int i = 0; i < caseAndExpectations.Length; ++i)
        {
            string[] caseAndExpectation = caseAndExpectations[i];

            if (caseAndExpectation.Length != 3)
            {
                throw new ArgumentException("Invalid test case.");
            }

            string caseText = caseAndExpectation[0];
            string expected = caseAndExpectation[1];
            string expectedPlainText = caseAndExpectation[2];

            InlineStyledTextComponent component = new InlineStyledTextComponent()
            {
                Text = caseText
            };

            IPromptComponent promptComponent = component;

            Assert.That(promptComponent.Render(), Is.EqualTo(expected), $"Line: {i + 1}\n     Case: {caseText}");

            component.UsePlainTextOnly();

            Assert.That(promptComponent.Render(), Is.EqualTo(expectedPlainText), $"Line: {i + 1}\n     Case: {caseText}");
        }
    }
}
