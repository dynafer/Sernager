using ServiceRunner.Runner.Utils;
using System.Text;

namespace ServiceRunner.Runner.Tests.Units.Utils;

public class ByteReaderTest
{
    private ByteReader mReader;

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
    }

    [Test]
    public void ReadInt32_ShouldReturnCorrectValue()
    {
        int expectedValue = BitConverter.ToInt32([1, 2, 3, 4], 0);
        int actualValue = mReader.ReadInt32();

        Assert.That(actualValue, Is.EqualTo(expectedValue));
    }

    [Test]
    public void ReadString_ShouldReturnCorrectValue()
    {
        string expectedString = "Test";
        mReader = new ByteReader(Encoding.UTF8.GetBytes(expectedString));

        string actualString = mReader.ReadString(expectedString.Length);

        Assert.That(actualString, Is.EqualTo(expectedString));
    }

    [Test]
    public void ReadBytes_WithZeroLength_ShouldReturnEmptyArray()
    {
        byte[] actualBytes = mReader.ReadBytes(0);

        Assert.That(actualBytes, Is.EqualTo(new byte[0]));
    }

    [Test]
    public void ReadBytes_WithLengthGreaterThanRemainingBytes_ShouldThrowException()
    {
        mReader.ReadBytes(4);

        Assert.Throws<ArgumentException>(() => mReader.ReadBytes(5));
    }

    [Test]
    public void ReadInt32_WithLessThanFourBytesRemaining_ShouldThrowException()
    {
        mReader.ReadBytes(5);

        Assert.Throws<ArgumentException>(() => mReader.ReadInt32());
    }

    [Test]
    public void ReadString_WithLengthGreaterThanRemainingBytes_ShouldThrowException()
    {
        mReader.ReadBytes(5);

        Assert.Throws<ArgumentException>(() => mReader.ReadString(4));
    }

    [Test]
    public void ReadBytes_AfterDispose_ShouldThrowException()
    {
        mReader.Dispose();

        Assert.Throws<ArgumentNullException>(() => mReader.ReadBytes(4));
    }

    [Test]
    public void ReadInt32_AfterDispose_ShouldThrowException()
    {
        mReader.Dispose();

        Assert.Throws<ArgumentNullException>(() => mReader.ReadInt32());
    }

    [Test]
    public void ReadString_AfterDispose_ShouldThrowException()
    {
        mReader.Dispose();

        Assert.Throws<ArgumentNullException>(() => mReader.ReadString(4));
    }
}
