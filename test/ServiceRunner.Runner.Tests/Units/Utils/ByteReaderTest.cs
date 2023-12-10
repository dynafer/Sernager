using ServiceRunner.Runner.Utils;
using System.Text;

namespace ServiceRunner.Runner.Tests.Units.Utils;

public class ByteReaderTest
{
    private ByteReader mReader { get; set; }

    [SetUp]
    public void Setup()
    {

        mReader = new ByteReader([1, 2, 3, 4, 5, 6, 7, 8]);
    }

    [TearDown]
    public void Teardown()
    {
        mReader.Dispose();
    }

    [Test]
    public void ReadBytes_ShouldReturnCorrectBytes()
    {
        byte[] expectedBytes = [1, 2, 3, 4];
        byte[] actualBytes = mReader.ReadBytes(4);

        Assert.That(actualBytes, Is.EqualTo(expectedBytes));
        Assert.That(mReader.Position, Is.EqualTo(4));
        Assert.That(mReader.Length, Is.EqualTo(8));
    }

    [Test]
    public void ReadInt32_ShouldReturnCorrectValue()
    {
        int expectedValue = BitConverter.ToInt32([1, 2, 3, 4], 0);
        int actualValue = mReader.ReadInt32();

        Assert.That(actualValue, Is.EqualTo(expectedValue));
        Assert.That(mReader.Position, Is.EqualTo(4));
        Assert.That(mReader.Length, Is.EqualTo(8));
    }

    [Test]
    public void ReadString_ShouldReturnCorrectValue()
    {
        string expectedString = "Test";
        mReader = new ByteReader(Encoding.UTF8.GetBytes(expectedString));

        string actualString = mReader.ReadString(expectedString.Length);

        Assert.That(actualString, Is.EqualTo(expectedString));
        Assert.That(mReader.Position, Is.EqualTo(expectedString.Length));
        Assert.That(mReader.Length, Is.EqualTo(expectedString.Length));
    }

    [Test]
    public void Skip_ShouldSkipBytes()
    {
        mReader.Skip(4);

        Assert.That(mReader.Position, Is.EqualTo(4));
        Assert.That(mReader.Length, Is.EqualTo(8));
    }

    [Test]
    public void ReadBytes_WithZeroLength_ShouldReturnEmptyArray()
    {
        byte[] actualBytes = mReader.ReadBytes(0);

        Assert.That(actualBytes, Is.EqualTo(new byte[0]));
        Assert.That(mReader.Position, Is.EqualTo(0));
        Assert.That(mReader.Length, Is.EqualTo(8));
    }

    [Test]
    public void ReadBytes_WithLengthGreaterThanRemainingBytes_ShouldThrowException()
    {
        mReader.ReadBytes(4);

        Assert.Throws<IndexOutOfRangeException>(() => mReader.ReadBytes(5));
    }

    [Test]
    public void ReadInt32_WithLessThanFourBytesRemaining_ShouldThrowException()
    {
        mReader.ReadBytes(5);

        Assert.Throws<IndexOutOfRangeException>(() => mReader.ReadInt32());
    }

    [Test]
    public void ReadString_WithLengthGreaterThanRemainingBytes_ShouldThrowException()
    {
        mReader.ReadBytes(5);

        Assert.Throws<IndexOutOfRangeException>(() => mReader.ReadString(4));
    }

    [Test]
    public void Skip_WithLengthGreaterThanRemainingBytes_ShouldThrowException()
    {
        mReader.ReadBytes(5);

        Assert.Throws<IndexOutOfRangeException>(() => mReader.Skip(4));
    }

    [Test]
    public void ReadBytes_AfterDispose_ShouldThrowException()
    {
        mReader.Dispose();

        Assert.Throws<ObjectDisposedException>(() => mReader.ReadBytes(4));
    }

    [Test]
    public void ReadInt32_AfterDispose_ShouldThrowException()
    {
        mReader.Dispose();

        Assert.Throws<ObjectDisposedException>(() => mReader.ReadInt32());
    }

    [Test]
    public void ReadString_AfterDispose_ShouldThrowException()
    {
        mReader.Dispose();

        Assert.Throws<ObjectDisposedException>(() => mReader.ReadString(4));
    }

    [Test]
    public void Skip_AfterDispose_ShouldThrowException()
    {
        mReader.Dispose();

        Assert.Throws<ObjectDisposedException>(() => mReader.Skip(4));
    }
}
