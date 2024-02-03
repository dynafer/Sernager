using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Cursors;

namespace Sernager.Terminal.Tests.Units.Prompts.Components;

internal sealed class CursorComponentTests
{
    [DatapointSource]
    private static readonly ECursorDirection[] DIRECTIONS = Enum.GetValues<ECursorDirection>();

    [Theory]
    public void AddCursor_ShouldAddCursor(ECursorDirection direction)
    {
        Assume.That(direction, Is.AnyOf(DIRECTIONS));

        CursorComponent component = new CursorComponent();
        int count = new Random().Next(1, 10);

        component.AddCursor(direction, count);

        Assert.That(component.Cursors, Has.Count.EqualTo(1));

        PromptCursor cursor = component.Cursors[0];

        Assert.That(cursor.Direction, Is.EqualTo(direction));
        Assert.That(cursor.Count, Is.EqualTo(count));

        switch (direction)
        {
            case ECursorDirection.Up:
                Assert.That(component.FianlY, Is.EqualTo(-count));
                break;
            case ECursorDirection.Down:
                Assert.That(component.FianlY, Is.EqualTo(count));
                break;
            case ECursorDirection.Left:
                Assert.That(component.FianlX, Is.EqualTo(-count));
                break;
            case ECursorDirection.Right:
                Assert.That(component.FianlX, Is.EqualTo(count));
                break;
            case ECursorDirection.HorizontalAbsolute:
                Assert.That(component.IsLastXAbsolute, Is.True);
                Assert.That(component.FianlX, Is.EqualTo(count));
                break;
        }
    }

    [Test]
    public void AddCursors_ShouldAddCursors()
    {
        CursorComponent component = new CursorComponent();
        (ECursorDirection, int)[] cursors =
        [
            (ECursorDirection.Up, new Random().Next(1, 10)),
            (ECursorDirection.Down, new Random().Next(1, 10)),
            (ECursorDirection.Left, new Random().Next(1, 10)),
            (ECursorDirection.Right, new Random().Next(1, 10)),
            (ECursorDirection.HorizontalAbsolute, new Random().Next(1, 10))
        ];

        component.AddCursors(cursors.Select(c => new PromptCursor(c.Item1, c.Item2)).ToArray());

        Assert.That(component.Cursors, Has.Count.EqualTo(cursors.Length));

        int expectedX = 0;
        int expectedY = 0;

        for (int i = 0; i < cursors.Length; ++i)
        {
            PromptCursor cursor = component.Cursors[i];
            (ECursorDirection direction, int count) = cursors[i];

            Assert.That(cursor.Direction, Is.EqualTo(direction));
            Assert.That(cursor.Count, Is.EqualTo(count));

            switch (direction)
            {
                case ECursorDirection.Up:
                    expectedY -= count;
                    break;
                case ECursorDirection.Down:
                    expectedY += count;
                    break;
                case ECursorDirection.Left:
                    expectedX -= count;
                    break;
                case ECursorDirection.Right:
                    expectedX += count;
                    break;
                case ECursorDirection.HorizontalAbsolute:
                    expectedX = count;
                    break;
            }
        }

        Assert.That(component.FianlX, Is.EqualTo(expectedX));
        Assert.That(component.FianlY, Is.EqualTo(expectedY));
        Assert.That(component.IsLastXAbsolute, Is.True);
    }

    [Test]
    public void Render_ShouldReturnString()
    {
        CursorComponent component = new CursorComponent();

        string expected = "";

        foreach (ECursorDirection direction in DIRECTIONS)
        {
            int count = new Random().Next(1, 10);

            component.AddCursor(direction, count);
            expected += getAnsiCodeBy(direction, count);
        }

        for (int i = 0; i < 10; ++i)
        {
            ECursorDirection direction = DIRECTIONS[new Random().Next(0, DIRECTIONS.Length)];
            int count = new Random().Next(1, 10);

            component.AddCursor(direction, count);
            expected += getAnsiCodeBy(direction, count);
        }

        // Will be no effect
        component.AddCursor((ECursorDirection)int.MaxValue, 10);

        IPromptComponent promptComponent = component;

        string result = promptComponent.Render();

        Assert.That(result, Is.EqualTo(expected));
    }

    private string getAnsiCodeBy(ECursorDirection direction, int count)
    {
        return direction switch
        {
            ECursorDirection.Up => AnsiCode.CursorUp(count),
            ECursorDirection.Down => AnsiCode.CursorDown(count),
            ECursorDirection.Left => AnsiCode.CursorLeft(count),
            ECursorDirection.Right => AnsiCode.CursorRight(count),
            ECursorDirection.HorizontalAbsolute => AnsiCode.CursorHorizontalAbsolute(count),
            _ => string.Empty
        };
    }
}
