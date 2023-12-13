using Sernager.Core.Tests.Fixtures;
using Sernager.Core.Utils;
using System.Text;

namespace Sernager.Core.Tests.Units.Utils;

public class ByteWriterTest : BaseFixture
{
    private ByteWriter mWriter { get; set; }
    private ByteWriter mWriterWithBytes { get; set; }
    private byte[] mWithBytes { get; set; } = [1, 2, 3, 4, 5];

    [SetUp]
    public void Setup()
    {
        mWriter = new ByteWriter();
        mWriterWithBytes = new ByteWriter(mWithBytes);
    }

    [TearDown]
    public void Teardown()
    {
        mWriter.Dispose();
        mWriterWithBytes.Dispose();
    }

    [Test]
    public void WriteBytesAndGetIntBack_ShouldReturnCorrectBytes()
    {
        int value = 12345;
        mWriter.WriteInt32(value);
        mWriterWithBytes.WriteInt32(value);

        byte[] expectedBytes = BitConverter.GetBytes(value);
        byte[] expectedBytesWithBytes = new byte[mWithBytes.Length + expectedBytes.Length];
        Buffer.BlockCopy(mWithBytes, 0, expectedBytesWithBytes, 0, mWithBytes.Length);
        Buffer.BlockCopy(expectedBytes, 0, expectedBytesWithBytes, mWithBytes.Length, expectedBytes.Length);

        Assert.That(mWriter.GetBytes(), Is.EqualTo(expectedBytes));
        Assert.That(mWriterWithBytes.GetBytes(), Is.EqualTo(expectedBytesWithBytes));
    }

    [Test]
    public void WriteStringAndGetIntBack_ShouldReturnCorrectBytes()
    {
        string value = "Test String";
        mWriter.WriteString(value);
        mWriterWithBytes.WriteString(value);

        byte[] expectedBytes = Encoding.UTF8.GetBytes(value);
        byte[] expectedBytesWithBytes = new byte[mWithBytes.Length + expectedBytes.Length];
        Buffer.BlockCopy(mWithBytes, 0, expectedBytesWithBytes, 0, mWithBytes.Length);
        Buffer.BlockCopy(expectedBytes, 0, expectedBytesWithBytes, mWithBytes.Length, expectedBytes.Length);

        Assert.That(mWriter.GetBytes(), Is.EqualTo(expectedBytes));
        Assert.That(mWriterWithBytes.GetBytes(), Is.EqualTo(expectedBytesWithBytes));
    }

    [Test]
    public void WriteBytesAndGetStringBack_ShouldReturnCorrectBytes()
    {
        byte[] bytes = [1, 2, 3, 4, 5];
        mWriter.WriteBytes(bytes);
        mWriterWithBytes.WriteBytes(bytes);

        byte[] expectedBytesWithBytes = new byte[mWithBytes.Length + bytes.Length];
        Buffer.BlockCopy(mWithBytes, 0, expectedBytesWithBytes, 0, mWithBytes.Length);
        Buffer.BlockCopy(bytes, 0, expectedBytesWithBytes, mWithBytes.Length, bytes.Length);

        Assert.That(mWriter.GetBytes(), Is.EqualTo(bytes));
        Assert.That(mWriterWithBytes.GetBytes(), Is.EqualTo(expectedBytesWithBytes));
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

        mWriterWithBytes.WriteInt32(intValue);
        mWriterWithBytes.WriteString(strValue);
        mWriterWithBytes.WriteBytes(bytesValue);

        byte[] expectedBytes = new byte[BitConverter.GetBytes(intValue).Length + Encoding.UTF8.GetBytes(strValue).Length + bytesValue.Length];
        Array.Copy(BitConverter.GetBytes(intValue), 0, expectedBytes, 0, BitConverter.GetBytes(intValue).Length);
        Array.Copy(Encoding.UTF8.GetBytes(strValue), 0, expectedBytes, BitConverter.GetBytes(intValue).Length, Encoding.UTF8.GetBytes(strValue).Length);
        Array.Copy(bytesValue, 0, expectedBytes, BitConverter.GetBytes(intValue).Length + Encoding.UTF8.GetBytes(strValue).Length, bytesValue.Length);

        byte[] expectedBytesWithBytes = new byte[mWithBytes.Length + expectedBytes.Length];
        Buffer.BlockCopy(mWithBytes, 0, expectedBytesWithBytes, 0, mWithBytes.Length);
        Buffer.BlockCopy(expectedBytes, 0, expectedBytesWithBytes, mWithBytes.Length, expectedBytes.Length);

        Assert.That(mWriter.GetBytes(), Is.EqualTo(expectedBytes));
        Assert.That(mWriterWithBytes.GetBytes(), Is.EqualTo(expectedBytesWithBytes));
    }

    [Test]
    public void WriteBytesThenWriteInt32_ShouldReturnCorrectBytes()
    {
        byte[] initialBytes = [1, 2, 3, 4, 5];
        int value = 12345;
        mWriter.WriteBytes(initialBytes);
        mWriter.WriteInt32(value);
        mWriterWithBytes.WriteBytes(initialBytes);
        mWriterWithBytes.WriteInt32(value);

        byte[] expectedBytes = new byte[initialBytes.Length + sizeof(int)];
        Buffer.BlockCopy(initialBytes, 0, expectedBytes, 0, initialBytes.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(value), 0, expectedBytes, initialBytes.Length, sizeof(int));

        byte[] expectedBytesWithBytes = new byte[mWithBytes.Length + expectedBytes.Length];
        Buffer.BlockCopy(mWithBytes, 0, expectedBytesWithBytes, 0, mWithBytes.Length);
        Buffer.BlockCopy(expectedBytes, 0, expectedBytesWithBytes, mWithBytes.Length, expectedBytes.Length);

        Assert.That(mWriter.GetBytes(), Is.EqualTo(expectedBytes));
        Assert.That(mWriterWithBytes.GetBytes(), Is.EqualTo(expectedBytesWithBytes));
    }

    [Test]
    public void WriteStringThenWriteBytes_ShouldReturnCorrectBytes()
    {
        string value = "Test String";
        byte[] bytes = [1, 2, 3, 4, 5];
        mWriter.WriteString(value);
        mWriter.WriteBytes(bytes);
        mWriterWithBytes.WriteString(value);
        mWriterWithBytes.WriteBytes(bytes);

        byte[] expectedBytes = new byte[Encoding.UTF8.GetByteCount(value) + bytes.Length];
        Buffer.BlockCopy(Encoding.UTF8.GetBytes(value), 0, expectedBytes, 0, Encoding.UTF8.GetByteCount(value));
        Buffer.BlockCopy(bytes, 0, expectedBytes, Encoding.UTF8.GetByteCount(value), bytes.Length);

        byte[] expectedBytesWithBytes = new byte[mWithBytes.Length + expectedBytes.Length];
        Buffer.BlockCopy(mWithBytes, 0, expectedBytesWithBytes, 0, mWithBytes.Length);
        Buffer.BlockCopy(expectedBytes, 0, expectedBytesWithBytes, mWithBytes.Length, expectedBytes.Length);

        Assert.That(mWriter.GetBytes(), Is.EqualTo(expectedBytes));
        Assert.That(mWriterWithBytes.GetBytes(), Is.EqualTo(expectedBytesWithBytes));
    }

    [Test]
    public void WriteInt32ThenWriteString_ShouldReturnCorrectBytes()
    {
        int value = 12345;
        string str = "Test String";
        mWriter.WriteInt32(value);
        mWriter.WriteString(str);
        mWriterWithBytes.WriteInt32(value);
        mWriterWithBytes.WriteString(str);

        byte[] expectedBytes = new byte[sizeof(int) + Encoding.UTF8.GetByteCount(str)];
        Buffer.BlockCopy(BitConverter.GetBytes(value), 0, expectedBytes, 0, sizeof(int));
        Buffer.BlockCopy(Encoding.UTF8.GetBytes(str), 0, expectedBytes, sizeof(int), Encoding.UTF8.GetByteCount(str));

        byte[] expectedBytesWithBytes = new byte[mWithBytes.Length + expectedBytes.Length];
        Buffer.BlockCopy(mWithBytes, 0, expectedBytesWithBytes, 0, mWithBytes.Length);
        Buffer.BlockCopy(expectedBytes, 0, expectedBytesWithBytes, mWithBytes.Length, expectedBytes.Length);

        Assert.That(mWriter.GetBytes(), Is.EqualTo(expectedBytes));
        Assert.That(mWriterWithBytes.GetBytes(), Is.EqualTo(expectedBytesWithBytes));
    }

    [Test]
    public void WriteBytes_WithNullBytes_ShouldThrowException()
    {
        Assert.Throws<NullReferenceException>(() => mWriter.WriteBytes(null!));
        Assert.Throws<NullReferenceException>(() => mWriterWithBytes.WriteBytes(null!));
    }

    [Test]
    public void WriteString_WithNullValue_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => mWriter.WriteString(null!));
        Assert.Throws<ArgumentNullException>(() => mWriterWithBytes.WriteString(null!));
    }

    [Test]
    public void WriteBytes_AfterDispose_ShouldThrowException()
    {
        mWriter.Dispose();
        mWriterWithBytes.Dispose();

        Assert.Throws<ObjectDisposedException>(() => mWriter.WriteBytes([1, 2, 3, 4, 5]));
        Assert.Throws<ObjectDisposedException>(() => mWriterWithBytes.WriteBytes([1, 2, 3, 4, 5]));
    }

    [Test]
    public void WriteInt32_AfterDispose_ShouldThrowException()
    {
        mWriter.Dispose();
        mWriterWithBytes.Dispose();

        Assert.Throws<ObjectDisposedException>(() => mWriter.WriteInt32(12345));
        Assert.Throws<ObjectDisposedException>(() => mWriterWithBytes.WriteInt32(12345));
    }

    [Test]
    public void WriteString_AfterDispose_ShouldThrowException()
    {
        mWriter.Dispose();
        mWriterWithBytes.Dispose();

        Assert.Throws<ObjectDisposedException>(() => mWriter.WriteString("Test String"));
        Assert.Throws<ObjectDisposedException>(() => mWriterWithBytes.WriteString("Test String"));
    }

    [Test]
    public void GetBytes_AfterDispose_ShouldThrowException()
    {
        mWriter.Dispose();
        mWriterWithBytes.Dispose();

        Assert.Throws<ObjectDisposedException>(() => mWriter.GetBytes());
        Assert.Throws<ObjectDisposedException>(() => mWriterWithBytes.GetBytes());
    }
}
