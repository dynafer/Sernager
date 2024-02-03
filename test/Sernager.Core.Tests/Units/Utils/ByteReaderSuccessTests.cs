using Sernager.Core.Utils;
using System.Text;

namespace Sernager.Core.Tests.Units.Utils;

internal sealed class ByteReaderSuccessTests
{
    private static readonly string TEMP_FILE_ALIAS = "ByteReader";
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

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        CaseUtil.DeleteTempFiles(TEMP_FILE_ALIAS);
    }

    [Test]
    public void ReadBytes_ShouldReturnBytes()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);

        using (ByteReader reader = new ByteReader(path))
        {
            byte[] result = reader.ReadBytes(bytes.Length);

            Assert.That(reader.Length, Is.EqualTo(bytes.Length));
            Assert.That(reader.Position, Is.EqualTo(bytes.Length));
            Assert.That(result, Is.EqualTo(bytes));
        }
    }

    [Test]
    public void TryReadBytes_ShouldReturnTrueAndOutBytes()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);

        using (ByteReader reader = new ByteReader(path))
        {
            byte[]? result;
            bool resultBool = reader.TryReadBytes(bytes.Length, out result);

            Assert.That(resultBool, Is.True);
            Assert.That(reader.Length, Is.EqualTo(bytes.Length));
            Assert.That(reader.Position, Is.EqualTo(bytes.Length));
            Assert.That(result, Is.EqualTo(bytes));
        }
    }

    [Test]
    public void TryReadInt32_ShouldReturnTrueAndOutInt32()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);

        using (ByteReader reader = new ByteReader(path))
        {
            int result;
            bool resultBool = reader.TryReadInt32(out result);

            Assert.That(resultBool, Is.True);
            Assert.That(reader.Length, Is.EqualTo(sizeof(int)));
            Assert.That(reader.Position, Is.EqualTo(sizeof(int)));
            Assert.That(result, Is.EqualTo(0x04030201));
        }

        int value = 20212223;
        path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, value);

        using (ByteReader reader = new ByteReader(path))
        {
            int result;
            bool resultBool = reader.TryReadInt32(out result);

            Assert.That(resultBool, Is.True);
            Assert.That(reader.Length, Is.EqualTo(sizeof(int)));
            Assert.That(reader.Position, Is.EqualTo(sizeof(int)));
            Assert.That(result, Is.EqualTo(value));
        }
    }

    [Test]
    public void TryReadString_ShouldReturnTrueAndOutString()
    {
        string value = "Hello, World!";
        byte[] bytes = Encoding.Default.GetBytes(value);
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);
        int readSum = 0;

        Action<ByteReader, string> assertString = (ByteReader reader, string str) =>
        {
            string? result;
            bool resultBool = reader.TryReadString(str.Length, out result);
            readSum += str.Length;

            Assert.That(resultBool, Is.True);
            Assert.That(reader.Length, Is.EqualTo(bytes.Length));

            Assert.That(reader.Position, Is.EqualTo(readSum));
            Assert.That(result, Is.EqualTo(str));
        };

        using (ByteReader reader = new ByteReader(path))
        {
            assertString(reader, "Hello");
            assertString(reader, ", ");
            assertString(reader, "World!");
        }

        bytes = Encoding.Default.GetBytes("{ \"Hello\": \"World!\" }");
        path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);
        readSum = 0;

        using (ByteReader reader = new ByteReader(path))
        {
            assertString(reader, "{ ");
            assertString(reader, "\"Hello\": ");
            assertString(reader, "\"World!\"");
            assertString(reader, " }");
        }
    }

    [Theory]
    public void TryReadString_ShouldReturnTrueAndOutString(Encoding encoding)
    {
        Assume.That(encoding, Is.AnyOf(ENCODING_LIST));

        string value = "Hello, World!";
        byte[] bytes = encoding.GetBytes(value);
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);
        int readSum = 0;

        Action<ByteReader, string> assertString = (ByteReader reader, string str) =>
        {
            string? result;
            int readLength = encoding.GetByteCount(str);
            bool resultBool = reader.TryReadString(encoding, readLength, out result);
            readSum += readLength;

            Assert.That(resultBool, Is.True);
            Assert.That(reader.Length, Is.EqualTo(bytes.Length));

            Assert.That(reader.Position, Is.EqualTo(readSum));
            Assert.That(result, Is.EqualTo(str));
        };

        using (ByteReader reader = new ByteReader(path))
        {
            assertString(reader, "Hello");
            assertString(reader, ", ");
            assertString(reader, "World!");
        }

        bytes = encoding.GetBytes("{ \"Hello\": \"World!\" }");
        path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);
        readSum = 0;

        using (ByteReader reader = new ByteReader(path))
        {
            assertString(reader, "{ ");
            assertString(reader, "\"Hello\": ");
            assertString(reader, "\"World!\"");
            assertString(reader, " }");
        }
    }

    [Test]
    public void TrySkip_With_ReadBytes_ShouldReturnTrue()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);

        using (ByteReader reader = new ByteReader(path))
        {
            byte[] result = reader.ReadBytes(2);

            Assert.That(reader.Length, Is.EqualTo(bytes.Length));

            Assert.That(reader.Position, Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(new byte[] { 0x01, 0x02 }));

            bool resultBool = reader.TrySkip(2);

            Assert.That(resultBool, Is.True);
            Assert.That(reader.Position, Is.EqualTo(4));

            result = reader.ReadBytes(2);

            Assert.That(reader.Position, Is.EqualTo(6));
            Assert.That(result, Is.EqualTo(new byte[] { 0x05, 0x06 }));
        }
    }
}
