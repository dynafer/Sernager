using Sernager.Core.Utils;
using System.Text;

namespace Sernager.Core.Tests.Units.Utils;

internal sealed class ByteReaderFailureTests : FailureFixture
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
    public void ReadBytes_ShouldThrow_WhenDisposed()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);

        using (ByteReader reader = new ByteReader(path))
        {
            reader.Dispose();

            TestNoneLevel(() => reader.ReadBytes(4), Is.Empty);
            TestExceptionLevel<ObjectDisposedException>(() => reader.ReadBytes(4));
        }
    }

    [Test]
    public void ReadBytes_ShouldThrow_WhenLengthIsLessThanOrEqualToZero()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);

        using (ByteReader reader = new ByteReader(path))
        {
            TestNoneLevel(() => reader.ReadBytes(0), Is.Empty);
            TestExceptionLevel<ArgumentException>(() => reader.ReadBytes(0));
        }

        using (ByteReader reader = new ByteReader(path))
        {
            TestNoneLevel(() => reader.ReadBytes(-1), Is.Empty);
            TestExceptionLevel<ArgumentException>(() => reader.ReadBytes(-1));
        }
    }

    [Test]
    public void ReadBytes_ShouldThrow_WhenPositionPlusLengthGreaterThanBytesLength()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);

        using (ByteReader reader = new ByteReader(path))
        {
            TestNoneLevel(() => reader.ReadBytes(5), Is.Empty);
            TestExceptionLevel<IndexOutOfRangeException>(() => reader.ReadBytes(5));
        }

        using (ByteReader reader = new ByteReader(path))
        {
            reader.ReadBytes(2);

            TestNoneLevel(() => reader.ReadBytes(3), Is.Empty);
            TestExceptionLevel<IndexOutOfRangeException>(() => reader.ReadBytes(3));
        }
    }

    [Test]
    public void TryReadBytes_ShouldReturnFalseAndOutNull_WhenDisposed()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);

        using (ByteReader reader = new ByteReader(path))
        {
            reader.Dispose();

            byte[]? result;
            bool bResult = reader.TryReadBytes(4, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }
    }

    [Test]
    public void TryReadBytes_ShouldReturnFalseAndOutNull_WhenLengthIsLessThanOrEqualToZero()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);
        byte[]? result;
        bool bResult;

        using (ByteReader reader = new ByteReader(path))
        {
            bResult = reader.TryReadBytes(0, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }

        using (ByteReader reader = new ByteReader(path))
        {
            bResult = reader.TryReadBytes(-1, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }
    }

    [Test]
    public void TryReadBytes_ShouldReturnFalseAndOutNull_WhenPositionPlusLengthGreaterThanBytesLength()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);
        byte[]? result;
        bool bResult;

        using (ByteReader reader = new ByteReader(path))
        {
            bResult = reader.TryReadBytes(5, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }

        using (ByteReader reader = new ByteReader(path))
        {
            reader.ReadBytes(2);

            bResult = reader.TryReadBytes(3, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }
    }

    [Test]
    public void TryReadInt32_ShouldReturnFalseAndOutNegativeOne_WhenDisposed()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);

        using (ByteReader reader = new ByteReader(path))
        {
            reader.Dispose();

            int result;
            bool bResult = reader.TryReadInt32(out result);

            Assert.That(result, Is.EqualTo(-1));
            Assert.That(bResult, Is.False);
        }
    }

    [Test]
    public void TryReadInt32_ShouldReturnFalseAndOutNegativeOne_WhenPositionPlusLengthGreaterThanBytesLength()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);
        int result;
        bool bResult;

        using (ByteReader reader = new ByteReader(path))
        {
            reader.TryReadInt32(out result);
            bResult = reader.TryReadInt32(out result);

            Assert.That(result, Is.EqualTo(-1));
            Assert.That(bResult, Is.False);
        }

        using (ByteReader reader = new ByteReader(path))
        {
            reader.ReadBytes(2);

            bResult = reader.TryReadInt32(out result);

            Assert.That(result, Is.EqualTo(-1));
            Assert.That(bResult, Is.False);
        }
    }

    [Test]
    public void TryReadString_ShouldReturnFalseAndOutNull_WhenDisposed()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);

        using (ByteReader reader = new ByteReader(path))
        {
            reader.Dispose();

            string? result;
            bool bResult = reader.TryReadString(4, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }
    }

    [Theory]
    public void TryReadString_ShouldReturnFalseAndOutNull_WhenDisposed(Encoding encoding)
    {
        Assume.That(encoding, Is.AnyOf(ENCODING_LIST));

        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);

        using (ByteReader reader = new ByteReader(path))
        {
            reader.Dispose();

            string? result;
            bool bResult = reader.TryReadString(encoding, 4, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }
    }

    [Test]
    public void TryReadString_ShouldReturnFalseAndOutNull_WhenLengthIsLessThanOrEqualToZero()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);
        string? result;
        bool bResult;

        using (ByteReader reader = new ByteReader(path))
        {
            bResult = reader.TryReadString(0, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }

        using (ByteReader reader = new ByteReader(path))
        {
            bResult = reader.TryReadString(-1, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }
    }

    [Theory]
    public void TryReadString_ShouldReturnFalseAndOutNull_WhenLengthIsLessThanOrEqualToZero(Encoding encoding)
    {
        Assume.That(encoding, Is.AnyOf(ENCODING_LIST));

        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);
        string? result;
        bool bResult;

        using (ByteReader reader = new ByteReader(path))
        {
            bResult = reader.TryReadString(encoding, 0, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }

        using (ByteReader reader = new ByteReader(path))
        {
            bResult = reader.TryReadString(encoding, -1, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }
    }

    [Test]
    public void TryReadString_ShouldReturnFalseAndOutNull_WhenPositionPlusLengthGreaterThanBytesLength()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);
        string? result;
        bool bResult;

        using (ByteReader reader = new ByteReader(path))
        {
            bResult = reader.TryReadString(5, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }

        using (ByteReader reader = new ByteReader(path))
        {
            reader.ReadBytes(2);

            bResult = reader.TryReadString(3, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }
    }

    [Theory]
    public void TryReadString_ShouldReturnFalseAndOutNull_WhenPositionPlusLengthGreaterThanBytesLength(Encoding encoding)
    {
        Assume.That(encoding, Is.AnyOf(ENCODING_LIST));

        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);
        string? result;
        bool bResult;

        using (ByteReader reader = new ByteReader(path))
        {
            bResult = reader.TryReadString(encoding, 5, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }

        using (ByteReader reader = new ByteReader(path))
        {
            reader.ReadBytes(2);

            bResult = reader.TryReadString(encoding, 3, out result);

            Assert.That(result, Is.Null);
            Assert.That(bResult, Is.False);
        }
    }

    [Test]
    public void TrySkip_ShouldReturnFalse_WhenDisposed()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);

        using (ByteReader reader = new ByteReader(path))
        {
            reader.Dispose();

            bool bResult = reader.TrySkip(4);

            Assert.That(bResult, Is.False);
        }
    }

    [Test]
    public void TrySkip_ShouldReturnFalse_WhenPositionPlusLengthGreaterThanBytesLength()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];
        string path = CaseUtil.CreateTempFile(TEMP_FILE_ALIAS, bytes);
        bool bResult;

        using (ByteReader reader = new ByteReader(path))
        {
            bResult = reader.TrySkip(5);

            Assert.That(bResult, Is.False);
        }

        using (ByteReader reader = new ByteReader(path))
        {
            reader.ReadBytes(2);

            bResult = reader.TrySkip(3);

            Assert.That(bResult, Is.False);
        }
    }
}
