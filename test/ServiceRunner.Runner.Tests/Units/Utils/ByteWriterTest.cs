using ServiceRunner.Runner.Utils;
using System.Text;

namespace ServiceRunner.Runner.Tests.Units.Utils;

public class ByteWriterTest
{
    private ByteWriter mWriter { get; set; }

    [SetUp]
    public void Setup()
    {
        mWriter = new ByteWriter();
    }

    [TearDown]
    public void Teardown()
    {
        mWriter.Dispose();
    }

    [Test]
    public void WriteBytesAndGetIntBack_ShouldReturnCorrectBytes()
    {
        int value = 12345;
        mWriter.WriteInt32(value);

        byte[] expectedBytes = BitConverter.GetBytes(value);
        Assert.That(mWriter.GetBytes(), Is.EqualTo(expectedBytes));
    }

    [Test]
    public void WriteStringAndGetIntBack_ShouldReturnCorrectBytes()
    {
        string value = "Test String";
        mWriter.WriteString(value);

        byte[] expectedBytes = Encoding.UTF8.GetBytes(value);
        Assert.That(mWriter.GetBytes(), Is.EqualTo(expectedBytes));
    }

    [Test]
    public void WriteBytesAndGetStringBack_ShouldReturnCorrectBytes()
    {
        byte[] bytes = [1, 2, 3, 4, 5];
        mWriter.WriteBytes(bytes);

        Assert.That(mWriter.GetBytes(), Is.EqualTo(bytes));
    }

    [Test]
    public void WriteMultipleTypesAndGetIntBack_ShouldReturnCorrectBytes()
    {
        int intValue = 12345;
        string strValue = "Test String";
        byte[] bytesValue = [1, 2, 3, 4, 5];

        mWriter.WriteInt32(intValue);
        mWriter.WriteString(strValue);
        mWriter.WriteBytes(bytesValue);

        byte[] expectedBytes = new byte[BitConverter.GetBytes(intValue).Length + Encoding.UTF8.GetBytes(strValue).Length + bytesValue.Length];
        Array.Copy(BitConverter.GetBytes(intValue), 0, expectedBytes, 0, BitConverter.GetBytes(intValue).Length);
        Array.Copy(Encoding.UTF8.GetBytes(strValue), 0, expectedBytes, BitConverter.GetBytes(intValue).Length, Encoding.UTF8.GetBytes(strValue).Length);
        Array.Copy(bytesValue, 0, expectedBytes, BitConverter.GetBytes(intValue).Length + Encoding.UTF8.GetBytes(strValue).Length, bytesValue.Length);

        Assert.That(mWriter.GetBytes(), Is.EqualTo(expectedBytes));
    }

    [Test]
    public void WriteBytesThenWriteInt32_ShouldReturnCorrectBytes()
    {
        byte[] initialBytes = [1, 2, 3, 4, 5];
        int value = 12345;
        mWriter.WriteBytes(initialBytes);
        mWriter.WriteInt32(value);

        byte[] expectedBytes = new byte[initialBytes.Length + sizeof(int)];
        Buffer.BlockCopy(initialBytes, 0, expectedBytes, 0, initialBytes.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(value), 0, expectedBytes, initialBytes.Length, sizeof(int));

        Assert.That(mWriter.GetBytes(), Is.EqualTo(expectedBytes));
    }

    [Test]
    public void WriteStringThenWriteBytes_ShouldReturnCorrectBytes()
    {
        string value = "Test String";
        byte[] bytes = [1, 2, 3, 4, 5];
        mWriter.WriteString(value);
        mWriter.WriteBytes(bytes);

        byte[] expectedBytes = new byte[Encoding.UTF8.GetByteCount(value) + bytes.Length];
        Buffer.BlockCopy(Encoding.UTF8.GetBytes(value), 0, expectedBytes, 0, Encoding.UTF8.GetByteCount(value));
        Buffer.BlockCopy(bytes, 0, expectedBytes, Encoding.UTF8.GetByteCount(value), bytes.Length);

        Assert.That(mWriter.GetBytes(), Is.EqualTo(expectedBytes));
    }

    [Test]
    public void WriteInt32ThenWriteString_ShouldReturnCorrectBytes()
    {
        int value = 12345;
        string str = "Test String";
        mWriter.WriteInt32(value);
        mWriter.WriteString(str);

        byte[] expectedBytes = new byte[sizeof(int) + Encoding.UTF8.GetByteCount(str)];
        Buffer.BlockCopy(BitConverter.GetBytes(value), 0, expectedBytes, 0, sizeof(int));
        Buffer.BlockCopy(Encoding.UTF8.GetBytes(str), 0, expectedBytes, sizeof(int), Encoding.UTF8.GetByteCount(str));

        Assert.That(mWriter.GetBytes(), Is.EqualTo(expectedBytes));
    }

    [Test]
    public void GetBytes_AfterDispose_ShouldBeNull()
    {
        mWriter.Dispose();

        Assert.That(mWriter.GetBytes(), Is.Null);
    }

    [Test]
    public void WriteBytes_WithNullBytes_ShouldThrowException()
    {
        Assert.Throws<NullReferenceException>(() => mWriter.WriteBytes(null!));
    }

    [Test]
    public void WriteString_WithNullValue_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => mWriter.WriteString(null!));
    }

    [Test]
    public void WriteBytes_AfterDispose_ShouldThrowException()
    {
        mWriter.Dispose();

        Assert.Throws<NullReferenceException>(() => mWriter.WriteBytes([1, 2, 3, 4, 5]));
    }

    [Test]
    public void WriteInt32_AfterDispose_ShouldThrowException()
    {
        mWriter.Dispose();

        Assert.Throws<NullReferenceException>(() => mWriter.WriteInt32(12345));
    }

    [Test]
    public void WriteString_AfterDispose_ShouldThrowException()
    {
        mWriter.Dispose();

        Assert.Throws<NullReferenceException>(() => mWriter.WriteString("Test String"));
    }
}
