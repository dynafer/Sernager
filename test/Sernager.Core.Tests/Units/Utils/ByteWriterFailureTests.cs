using Sernager.Core.Tests.Fixtures;
using Sernager.Core.Utils;

namespace Sernager.Core.Tests.Units.Utils;

public class ByteWriterFailureTests : FailureFixture
{
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
