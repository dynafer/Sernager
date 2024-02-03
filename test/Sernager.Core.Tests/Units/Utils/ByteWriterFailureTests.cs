using Sernager.Core.Utils;
using System.Text;

namespace Sernager.Core.Tests.Units.Utils;

internal sealed class ByteWriterFailureTests : FailureFixture
{
    [DatapointSource]
    private static readonly Encoding[] ENCODING_LIST =
    [
        Encoding.UTF8,
        Encoding.Unicode,
        Encoding.BigEndianUnicode,
        Encoding.UTF32,
        Encoding.ASCII,
        Encoding.Default,
    ];

    [Test]
    public void WriteBytes_ShouldThrow_WhenDisposed()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];

        using (ByteWriter writer = new ByteWriter())
        {
            writer.Dispose();

            TestNoneLevel(() => writer.WriteBytes(bytes), Is.Null);
            TestExceptionLevel<ObjectDisposedException>(() => writer.WriteBytes(bytes));
        }
    }

    [Test]
    public void WriteInt32_ShouldThrow_WhenDisposed()
    {
        using (ByteWriter writer = new ByteWriter())
        {
            writer.Dispose();

            TestNoneLevel(() => writer.WriteInt32(0), Is.Null);
            TestExceptionLevel<ObjectDisposedException>(() => writer.WriteInt32(0));
        }
    }

    [Test]
    public void WriteString_ShouldThrow_WhenDisposed()
    {
        using (ByteWriter writer = new ByteWriter())
        {
            writer.Dispose();

            TestNoneLevel(() => writer.WriteString(string.Empty), Is.Null);
            TestExceptionLevel<ObjectDisposedException>(() => writer.WriteString(string.Empty));
        }
    }

    [Theory]
    public void WriteString_ShouldThrow_WhenDisposed(Encoding encoding)
    {
        Assume.That(encoding, Is.AnyOf(ENCODING_LIST));

        using (ByteWriter writer = new ByteWriter())
        {
            writer.Dispose();

            TestNoneLevel(() => writer.WriteString(encoding, string.Empty), Is.Null);
            TestExceptionLevel<ObjectDisposedException>(() => writer.WriteString(encoding, string.Empty));
        }
    }

    [Test]
    public void GetBytes_ShouldThrow_WhenDisposed()
    {
        using (ByteWriter writer = new ByteWriter())
        {
            writer.Dispose();

            TestNoneLevel(writer.GetBytes, Is.Empty);
            TestExceptionLevel<ObjectDisposedException>(() => writer.GetBytes());
        }
    }
}
