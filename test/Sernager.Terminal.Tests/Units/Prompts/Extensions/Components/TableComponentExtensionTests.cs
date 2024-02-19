using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Tables;
using Sernager.Terminal.Prompts.Extensions.Components;

namespace Sernager.Terminal.Tests.Units.Prompts.Extensions.Components;

internal sealed class TableComponentExtensionTests
{
    [Test]
    public void AddRows_ShouldAddRows_WhenParamsAreRowInstances()
    {
        TableComponent component = new TableComponent();

        component.AddRows(
            new Row("Row 1 item 1", "Row 1 item 2", "Row 1 item 3"),
            new Row("Row 2 item 1", "Row 2 item 2", "Row 2 item 3"),
            new Row("Row 3 item 1", "Row 3 item 2", "Row 3 item 3")
        );

        Assert.That(component.Rows.Count, Is.EqualTo(3));
    }

    [Test]
    public void AddRows_ShouldAddRows_WhenParamsAreColumnArrays()
    {
        TableComponent component = new TableComponent();

        component.AddRows(
            [new Column("Row 1 item 1", 2), new Column("Row 1 item 2", 2), new Column("Row 1 item 3", 2)],
            [new Column("Row 2 item 1", 2), new Column("Row 2 item 2", 2), new Column("Row 2 item 3", 2)],
            [new Column("Row 3 item 1", 2), new Column("Row 3 item 2", 2), new Column("Row 3 item 3", 2)]
        );

        Assert.That(component.Rows.Count, Is.EqualTo(3));
    }

    [Test]
    public void AddRows_ShouldAddRows_WhenParamsAreTextComponentArrays()
    {
        TableComponent component = new TableComponent();

        component.AddRows(
            [new TextComponent().SetText("Row 1 item 1"), new TextComponent().SetText("Row 1 item 2"), new TextComponent().SetText("Row 1 item 3")],
            [new TextComponent().SetText("Row 2 item 1"), new TextComponent().SetText("Row 2 item 2"), new TextComponent().SetText("Row 2 item 3")],
            [new TextComponent().SetText("Row 3 item 1"), new TextComponent().SetText("Row 3 item 2"), new TextComponent().SetText("Row 3 item 3")]
        );

        Assert.That(component.Rows.Count, Is.EqualTo(3));
    }

    [Test]
    public void AddRows_ShouldAddRows_WhenParamsAreStringArrays()
    {
        TableComponent component = new TableComponent();

        component.AddRows(
            ["Row 1 item 1", "Row 1 item 2", "Row 1 item 3"],
            ["Row 2 item 1", "Row 2 item 2", "Row 2 item 3"],
            ["Row 3 item 1", "Row 3 item 2", "Row 3 item 3"]
        );

        Assert.That(component.Rows.Count, Is.EqualTo(3));
    }
}
