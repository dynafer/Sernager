using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Tables;

namespace Sernager.Terminal.Tests.Units.Prompts.Components;

internal sealed class TableComponentTests
{
    [Test]
    public void UseBorder_ShouldUseBorderOption()
    {
        TableComponent component = new TableComponent();

        Assert.That(PrivateUtil.GetFieldValue<bool>(component, "mbUseBorder"), Is.False);

        component.UseBorder();

        Assert.That(PrivateUtil.GetFieldValue<bool>(component, "mbUseBorder"), Is.True);
    }

    [Test]
    public void UseHeader_ShouldUseHeaderOption()
    {
        TableComponent component = new TableComponent();

        Assert.That(PrivateUtil.GetFieldValue<bool>(component, "mbUseHeader"), Is.False);

        component.UseHeader();

        Assert.That(PrivateUtil.GetFieldValue<bool>(component, "mbUseHeader"), Is.True);
    }

    [Test]
    public void Render_ShouldThrow_WhenRowsAreDifferentLength()
    {
        TableComponent component = new TableComponent();

        component.UseHeader();

        component.Rows.AddRange([
            new Row("Row 1 item 1", "Row 1 item 2", "Row 1 item 3"),
            new Row("Row 2 item 1", "Row 2 item 2"),
            new Row(new Column("Row 3", 4))
        ]);

        IPromptComponent promptComponent = component;

        Assert.Throws<InvalidOperationException>(() => promptComponent.Render());
    }

    [Test]
    public void Render_ShouldReturnEmpty_WhenRowsAreEmpty()
    {
        TableComponent component = new TableComponent();

        IPromptComponent promptComponent = component;

        string result = promptComponent.Render();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Render_ShouldReturnRenderedText_WithHeaderOption()
    {
        TableComponent component = new TableComponent();

        component.UseHeader();

        component.Rows.AddRange(createCommonRows());

        IPromptComponent promptComponent = component;

        string result = promptComponent.Render().Replace("\r\n", "\n");
        string expected = CaseUtil.ReadString("Prompts.Tables.Header", "txt").Replace("\r\n", "\n");

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Render_ShouldReturnRenderedText_WithBorderOption()
    {
        TableComponent component = new TableComponent();

        component.UseBorder();

        Row[] rows = createCommonRows();

        component.Rows.AddRange(rows.Skip(1));

        IPromptComponent promptComponent = component;

        string result = promptComponent.Render().Replace("\r\n", "\n");
        string expected = CaseUtil.ReadString("Prompts.Tables.Border", "txt").Replace("\r\n", "\n");

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Render_ShouldReturnRenderedText_WithHeaderAndBorderOption()
    {
        TableComponent component = new TableComponent();

        component.UseHeader()
                 .UseBorder();

        component.Rows.AddRange(createCommonRows());

        IPromptComponent promptComponent = component;

        string result = promptComponent.Render().Replace("\r\n", "\n");
        string expected = CaseUtil.ReadString("Prompts.Tables.HeaderAndBorder", "txt").Replace("\r\n", "\n");

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Render_ShouldReturnRenderedText_WithFirstRowColspan()
    {
        TableComponent component = new TableComponent();

        component.UseHeader()
                 .UseBorder();

        Row[] rows = createCommonRows();

        rows[1] = new Row(
            new Column("Row 1 item 1 with colspan", 2),
            new Column("Row 1 item 3")
        );

        component.Rows.AddRange(rows);

        IPromptComponent promptComponent = component;

        string result = promptComponent.Render().Replace("\r\n", "\n");
        string expected = CaseUtil.ReadString("Prompts.Tables.FirstRowColspan", "txt").Replace("\r\n", "\n");

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Render_ShouldReturnRenderedText_WithMiddleRowColspan()
    {
        TableComponent component = new TableComponent();

        component.UseHeader()
                 .UseBorder();

        Row[] rows = createCommonRows();

        rows[3] = new Row(
            new Column("Row 3 item 1 with colspan", 2),
            new Column("Row 3 item 3")
        );

        component.Rows.AddRange(rows);

        IPromptComponent promptComponent = component;

        string result = promptComponent.Render().Replace("\r\n", "\n");
        string expected = CaseUtil.ReadString("Prompts.Tables.MiddleRowColspan", "txt").Replace("\r\n", "\n");

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Render_ShouldReturnRenderedText_WithLastRowColspan()
    {
        TableComponent component = new TableComponent();

        component.UseHeader()
                 .UseBorder();

        Row[] rows = createCommonRows();

        rows[4] = new Row(
            new Column("Row 4 item 1 with colspan", 2),
            new Column("Row 4 item 3")
        );

        component.Rows.AddRange(rows);

        IPromptComponent promptComponent = component;

        string result = promptComponent.Render().Replace("\r\n", "\n");
        string expected = CaseUtil.ReadString("Prompts.Tables.LastRowColspan", "txt").Replace("\r\n", "\n");

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Render_ShouldReturnRenderedText_WithMultipleColspan()
    {
        TableComponent component = new TableComponent();

        component.UseHeader()
                 .UseBorder();

        Row[] rows = createCommonRows();

        rows[1] = new Row(
            new Column("Row 1 item 1 with colspan", 2),
            new Column("Row 1 item 3")
        );

        rows[3] = new Row(
            new Column("Row 3 with colspan", 3)
        );

        rows[4] = new Row(
            new Column("Row 4 item 1"),
            new Column("Row 4 item 2 with colspan", 2)
        );

        component.Rows.AddRange(rows);

        IPromptComponent promptComponent = component;

        string result = promptComponent.Render().Replace("\r\n", "\n");
        string expected = CaseUtil.ReadString("Prompts.Tables.MultipleColspan", "txt").Replace("\r\n", "\n");

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Render_ShouldReturnRenderedText_WithMultipleColspan_WithoutBorder()
    {
        TableComponent component = new TableComponent();

        component.UseHeader();

        Row[] rows = createCommonRows();

        rows[1] = new Row(
            new Column("Row 1 item 1 with colspan", 2),
            new Column("Row 1 item 3")
        );

        rows[3] = new Row(
            new Column("Row 3 with colspan", 3)
        );

        rows[4] = new Row(
            new Column("Row 4 item 1"),
            new Column("Row 4 item 2 with colspan", 2)
        );

        component.Rows.AddRange(rows);

        IPromptComponent promptComponent = component;

        string result = promptComponent.Render().Replace("\r\n", "\n");
        string expected = CaseUtil.ReadString("Prompts.Tables.MultipleColspanWithoutBorder", "txt").Replace("\r\n", "\n");

        Assert.That(result, Is.EqualTo(expected));
    }

    private Row[] createCommonRows()
    {
        Row header = new Row();
        header.AddColumn(new TextComponent { Text = "Header 1" })
              .AddColumn(new TextComponent { Text = "Header 2" })
              .AddColumn(new TextComponent { Text = "Header 3" });

        Row row1 = new Row();
        row1.AddColumns([
            new TextComponent { Text = "Row 1 item 1" },
            new TextComponent { Text = "Row 1 item 2" },
            new TextComponent { Text = "Row 1 item 3" }
        ]);

        Row row2 = new Row(
            new TextComponent { Text = "Row 2 item 1" },
            new TextComponent { Text = "Row 2 item 2" },
            new TextComponent { Text = "Row 2 item 3" }
        );

        Row row3 = new Row("Row 3 item 1", "Row 3 item 2", "Row 3 item 3");
        Row row4 = new Row("Row 4 item 1", "Row 4 item 2", "Row 4 item 3");

        return [header, row1, row2, row3, row4];
    }
}