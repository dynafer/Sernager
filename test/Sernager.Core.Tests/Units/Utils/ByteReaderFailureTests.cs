using Sernager.Core.Tests.Fixtures;
using Sernager.Core.Utils;
using System.Text;

namespace Sernager.Core.Tests.Units.Utils;

public class ByteReaderFailureTests : FailureFixture
{
    [Test]
    public void ReadBytes_ShouldThrow_WhenDisposed()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];

        using (ByteReader reader = new ByteReader(bytes))
        {
            reader.Dispose();

            TestNoneLevel(() => reader.ReadBytes(4), Is.Empty);
            TestExceptionLevel<ObjectDisposedException>(() => reader.ReadBytes(4));
        }
    }

    [Test]
    public void ReadBytes_ShouldThrow_WhenPositionPlusLengthGreaterThanBytesLength()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];

        using (ByteReader reader = new ByteReader(bytes))
        {
            TestNoneLevel(() => reader.ReadBytes(5), Is.Empty);
            TestExceptionLevel<IndexOutOfRangeException>(() => reader.ReadBytes(5));
        }

        using (ByteReader reader = new ByteReader(bytes))
        {
            reader.ReadBytes(2);

            TestNoneLevel(() => reader.ReadBytes(3), Is.Empty);
            TestExceptionLevel<IndexOutOfRangeException>(() => reader.ReadBytes(3));
        }
    }

    [Test]
    public void ReadInt32_ShouldThrow_WhenDisposed()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];

        using (ByteReader reader = new ByteReader(bytes))
        {
            reader.Dispose();

            TestNoneLevel(reader.ReadInt32, Is.Zero);
            TestExceptionLevel<ObjectDisposedException>(() => reader.ReadInt32());
        }
    }

    [Test]
    public void ReadInt32_ShouldThrow_WhenPositionPlusLengthGreaterThanBytesLength()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];

        using (ByteReader reader = new ByteReader(bytes))
        {
            reader.ReadInt32();

            TestNoneLevel(reader.ReadInt32, Is.Zero);
            TestExceptionLevel<IndexOutOfRangeException>(() => reader.ReadInt32());
        }
    }

    [Test]
    public void ReadString_ShouldThrow_WhenDisposed()
    {
        string value = "Hello, World!";
        byte[] bytes = Encoding.UTF8.GetBytes(value);

        using (ByteReader reader = new ByteReader(bytes))
        {
            reader.Dispose();

            TestNoneLevel(() => reader.ReadString(4), Is.Empty);
            TestExceptionLevel<ObjectDisposedException>(() => reader.ReadString(4));
        }
    }

    [Test]
    public void ReadString_ShouldThrow_WhenPositionPlusLengthGreaterThanBytesLength()
    {
        string value = "Hello, World!";
        byte[] bytes = Encoding.UTF8.GetBytes(value);

        using (ByteReader reader = new ByteReader(bytes))
        {
            reader.ReadString(value.Length);

            TestNoneLevel(() => reader.ReadString(1), Is.Empty);
            TestExceptionLevel<IndexOutOfRangeException>(() => reader.ReadString(1));
        }
    }

    [Test]
    public void Skip_ShouldThrow_WhenDisposed()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];

        using (ByteReader reader = new ByteReader(bytes))
        {
            reader.Dispose();

            TestNoneLevel(() => reader.Skip(4));
            TestExceptionLevel<ObjectDisposedException>(() => reader.Skip(4));
        }
    }

    [Test]
    public void Skip_ShouldThrow_WhenPositionPlusLengthGreaterThanBytesLength()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];

        using (ByteReader reader = new ByteReader(bytes))
        {
            reader.Skip(4);

            TestNoneLevel(() => reader.Skip(1));
            TestExceptionLevel<IndexOutOfRangeException>(() => reader.Skip(1));
        }
    }
}
