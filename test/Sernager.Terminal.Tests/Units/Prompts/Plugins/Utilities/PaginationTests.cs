using Sernager.Terminal.Prompts.Plugins.Utilities;
using System.Diagnostics;

namespace Sernager.Terminal.Tests.Units.Prompts.Plugins.Utilities;

internal sealed class PaginationTests
{
    [Test]
    public void GetRange_ShouldReturnRange_WhenPageSizeIsMoreThanTotal()
    {
        Pagination pagination = new Pagination
        {
            PageSize = 5,
            Total = 4,
        };

        PaginationRange range = pagination.GetRange();

        Assert.That(range.Start, Is.Zero);
        Assert.That(range.End, Is.EqualTo(4));
        Assert.That(range.Prev, Is.Zero);
        Assert.That(range.Next, Is.Zero);

        PrivateUtil.SetPropertyValue(pagination, "Offset", 4);

        range = pagination.GetRange();

        Assert.That(range.Start, Is.Zero);
        Assert.That(range.End, Is.EqualTo(4));
        Assert.That(range.Prev, Is.Zero);
        Assert.That(range.Next, Is.Zero);
    }

    [Test]
    public void GetRange_ShouldReturnRange_WhenPageSizeIsLessThanTotal()
    {
        Pagination pagination = new Pagination
        {
            PageSize = 5,
            Total = 10,
        };

        PaginationRange range = pagination.GetRange();

        testPaginationRange(range, 0, 2, 2, 0);

        PrivateUtil.SetPropertyValue(pagination, "Offset", 1);
        range = pagination.GetRange();

        testPaginationRange(range, 0, 3, 1, 0);

        PrivateUtil.SetPropertyValue(pagination, "Offset", 2);
        range = pagination.GetRange();

        testPaginationRange(range, 0, 4, 0, 0);

        PrivateUtil.SetPropertyValue(pagination, "Offset", 5);
        range = pagination.GetRange();

        testPaginationRange(range, 3, 7, 0, 0);

        PrivateUtil.SetPropertyValue(pagination, "Offset", 9);
        range = pagination.GetRange();

        testPaginationRange(range, 7, 10, 0, 1);

        PrivateUtil.SetPropertyValue(pagination, "Offset", 10);
        range = pagination.GetRange();

        testPaginationRange(range, 8, 10, 0, 2);
    }

    [Test]
    public void Home_ShouldSetOffsetToZero()
    {
        Pagination pagination = new Pagination
        {
            PageSize = 5,
            Total = 10,
        };

        PrivateUtil.SetPropertyValue(pagination, "Offset", 5);

        Assert.That(pagination.Offset, Is.EqualTo(5));

        pagination.Home();

        Assert.That(pagination.Offset, Is.Zero);
    }

    [Test]
    public void Prev_ShouldDecrementOffset_WhenOffsetIsGreaterThanZero()
    {
        Pagination pagination = new Pagination
        {
            PageSize = 5,
            Total = 10,
        };

        PrivateUtil.SetPropertyValue(pagination, "Offset", 5);

        Assert.That(pagination.Offset, Is.EqualTo(5));

        pagination.Prev();

        Assert.That(pagination.Offset, Is.EqualTo(4));
    }

    [Test]
    public void Prev_ShouldSetOffsetToLastIndex_WhenOffsetIsZero()
    {
        Pagination pagination = new Pagination
        {
            PageSize = 5,
            Total = 10,
        };

        PrivateUtil.SetPropertyValue(pagination, "Offset", 0);

        Assert.That(pagination.Offset, Is.Zero);

        pagination.Prev();

        Assert.That(pagination.Offset, Is.EqualTo(9));
    }

    [Test]
    public void Next_ShouldIncrementOffset_WhenOffsetIsLessThanLastIndex()
    {
        Pagination pagination = new Pagination
        {
            PageSize = 5,
            Total = 10,
        };

        PrivateUtil.SetPropertyValue(pagination, "Offset", 5);

        Assert.That(pagination.Offset, Is.EqualTo(5));

        pagination.Next();

        Assert.That(pagination.Offset, Is.EqualTo(6));
    }

    [Test]
    public void Next_ShouldSetOffsetToZero_WhenOffsetIsLastIndex()
    {
        Pagination pagination = new Pagination
        {
            PageSize = 5,
            Total = 10,
        };

        PrivateUtil.SetPropertyValue(pagination, "Offset", 9);

        Assert.That(pagination.Offset, Is.EqualTo(9));

        pagination.Next();

        Assert.That(pagination.Offset, Is.Zero);
    }

    [StackTraceHidden]
    private void testPaginationRange(PaginationRange range, int start, int end, int prev, int next)
    {
        Assert.That(range.Start, Is.EqualTo(start));
        Assert.That(range.End, Is.EqualTo(end));
        Assert.That(range.Prev, Is.EqualTo(prev));
        Assert.That(range.Next, Is.EqualTo(next));
    }
}
