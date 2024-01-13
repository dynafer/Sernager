using Sernager.Core.Utils;
using System.Text;

namespace Sernager.Core.Tests.Units.Utils;

public class ByteReaderSuccessTests
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
    public void ReadBytes_ShouldReturnBytes()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];

        using (ByteReader reader = new ByteReader(bytes))
        {
            byte[] result = reader.ReadBytes(bytes.Length);

            Assert.That(reader.Length, Is.EqualTo(bytes.Length));
            Assert.That(reader.Position, Is.EqualTo(bytes.Length));
            Assert.That(result, Is.EqualTo(bytes));
        }
    }

    [Test]
    public void TryReadInt32_ShouldReturnInt32()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];

        using (ByteReader reader = new ByteReader(bytes))
        {
            int result;

            if (!reader.TryReadInt32(out result))
            {
                Assert.Fail();
            }

            Assert.That(reader.Length, Is.EqualTo(sizeof(int)));
            Assert.That(reader.Position, Is.EqualTo(sizeof(int)));
            Assert.That(result, Is.EqualTo(0x04030201));
        }

        int value = 20212223;
        bytes = BitConverter.GetBytes(value);

        using (ByteReader reader = new ByteReader(bytes))
        {
            int result;

            if (!reader.TryReadInt32(out result))
            {
                Assert.Fail();
            }

            Assert.That(reader.Length, Is.EqualTo(sizeof(int)));
            Assert.That(reader.Position, Is.EqualTo(sizeof(int)));
            Assert.That(result, Is.EqualTo(value));
        }
    }

    [Test]
    public void TryReadString_ShouldReturnString()
    {
        string value = "Hello, World!";
        byte[] bytes = Encoding.Default.GetBytes(value);
        string? result;
        int readSum = 0;

        Action<ByteReader, string> assertString = (ByteReader reader, string str) =>
        {
            readSum += str.Length;
            if (!reader.TryReadString(str.Length, out result))
            {
                Assert.Fail();
            }

            Assert.That(reader.Length, Is.EqualTo(bytes.Length));

            Assert.That(reader.Position, Is.EqualTo(readSum));
            Assert.That(result, Is.EqualTo(str));
        };

        using (ByteReader reader = new ByteReader(bytes))
        {
            assertString(reader, "Hello");
            assertString(reader, ", ");
            assertString(reader, "World!");
        }

        value = "{ \"Hello\": \"World!\" }";
        bytes = Encoding.Default.GetBytes(value);
        readSum = 0;

        using (ByteReader reader = new ByteReader(bytes))
        {
            assertString(reader, "{ ");
            assertString(reader, "\"Hello\": ");
            assertString(reader, "\"World!\"");
            assertString(reader, " }");
        }
    }

    [Theory]
    public void TryReadString_ShouldReturnString(Encoding encoding)
    {
        Assume.That(encoding, Is.AnyOf(ENCODING_LIST));

        string value = "Hello, World!";
        byte[] bytes = encoding.GetBytes(value);
        string? result;
        int readLength;
        int readSum = 0;

        Action<ByteReader, string> assertString = (ByteReader reader, string str) =>
        {
            readLength = encoding.GetByteCount(str);
            readSum += readLength;
            if (!reader.TryReadString(encoding, readLength, out result))
            {
                Assert.Fail();
            }

            Assert.That(reader.Length, Is.EqualTo(bytes.Length));

            Assert.That(reader.Position, Is.EqualTo(readSum));
            Assert.That(result, Is.EqualTo(str));
        };

        using (ByteReader reader = new ByteReader(bytes))
        {
            assertString(reader, "Hello");
            assertString(reader, ", ");
            assertString(reader, "World!");
        }

        value = "{ \"Hello\": \"World!\" }";
        bytes = encoding.GetBytes(value);
        readSum = 0;

        using (ByteReader reader = new ByteReader(bytes))
        {
            assertString(reader, "{ ");
            assertString(reader, "\"Hello\": ");
            assertString(reader, "\"World!\"");
            assertString(reader, " }");
        }
    }

    [Test]
    public void TrySkip_With_ReadBytes_ShouldReturnNextSkippedBytes()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06];

        using (ByteReader reader = new ByteReader(bytes))
        {
            byte[] result = reader.ReadBytes(2);

            Assert.That(reader.Length, Is.EqualTo(bytes.Length));

            Assert.That(reader.Position, Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(new byte[] { 0x01, 0x02 }));

            if (!reader.TrySkip(2))
            {
                Assert.Fail();
            }

            Assert.That(reader.Position, Is.EqualTo(4));

            result = reader.ReadBytes(2);

            Assert.That(reader.Position, Is.EqualTo(6));
            Assert.That(result, Is.EqualTo(new byte[] { 0x05, 0x06 }));
        }
    }
}
