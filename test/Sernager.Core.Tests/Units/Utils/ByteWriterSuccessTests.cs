using Sernager.Core.Utils;
using System.Text;

namespace Sernager.Core.Tests.Units.Utils;

internal sealed class ByteWriterSuccessTests
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
    public void WriteBytes_ShouldWriteProperly()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];

        using (ByteWriter writer = new ByteWriter())
        {
            writer.WriteBytes(bytes);

            byte[] result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length));
            Assert.That(result, Is.EqualTo(bytes));

            writer.WriteBytes(bytes)
                .WriteBytes(bytes)
                .WriteBytes(bytes);

            result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length * 4));
            Assert.That(result, Is.EqualTo(bytes.Concat(bytes).Concat(bytes).Concat(bytes)));
        }

        using (ByteWriter writer = new ByteWriter(bytes))
        {
            byte[] result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length));
            Assert.That(result, Is.EqualTo(bytes));

            writer.WriteBytes(bytes)
                .WriteBytes(bytes)
                .WriteBytes(bytes);

            result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length * 4));
            Assert.That(result, Is.EqualTo(bytes.Concat(bytes).Concat(bytes).Concat(bytes)));
        }
    }

    [Test]
    public void WriteInt32_ShouldWriteProperly()
    {
        int value = 20212223;
        byte[] bytes = BitConverter.GetBytes(value);

        using (ByteWriter writer = new ByteWriter())
        {
            writer.WriteInt32(value);

            byte[] result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length));
            Assert.That(result, Is.EqualTo(bytes));

            writer.WriteInt32(value)
                .WriteInt32(value)
                .WriteInt32(value);

            result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length * 4));
            Assert.That(result, Is.EqualTo(bytes.Concat(bytes).Concat(bytes).Concat(bytes)));
        }

        using (ByteWriter writer = new ByteWriter(bytes))
        {
            byte[] result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length));
            Assert.That(result, Is.EqualTo(bytes));

            writer.WriteInt32(value)
                .WriteInt32(value)
                .WriteInt32(value);

            result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length * 4));
            Assert.That(result, Is.EqualTo(bytes.Concat(bytes).Concat(bytes).Concat(bytes)));
        }
    }

    [Test]
    public void WriteString_ShouldWriteProperly()
    {
        string value = "Hello, World!";
        byte[] bytes = Encoding.Default.GetBytes(value);

        using (ByteWriter writer = new ByteWriter())
        {
            writer.WriteString(value);

            byte[] result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length));
            Assert.That(result, Is.EqualTo(bytes));

            writer.WriteString(value)
                .WriteString(value)
                .WriteString(value);

            result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length * 4));
            Assert.That(result, Is.EqualTo(bytes.Concat(bytes).Concat(bytes).Concat(bytes)));
        }

        using (ByteWriter writer = new ByteWriter(bytes))
        {
            byte[] result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length));
            Assert.That(result, Is.EqualTo(bytes));

            writer.WriteString(value)
                .WriteString(value)
                .WriteString(value);

            result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length * 4));
            Assert.That(result, Is.EqualTo(bytes.Concat(bytes).Concat(bytes).Concat(bytes)));
        }
    }

    [Theory]
    public void WriteString_ShouldWriteProperly(Encoding encoding)
    {
        Assume.That(encoding, Is.AnyOf(ENCODING_LIST));

        string value = "Hello, World!";
        byte[] bytes = encoding.GetBytes(value);

        using (ByteWriter writer = new ByteWriter())
        {
            writer.WriteString(encoding, value);

            byte[] result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length));
            Assert.That(result, Is.EqualTo(bytes));

            writer.WriteString(encoding, value)
                .WriteString(encoding, value)
                .WriteString(encoding, value);

            result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length * 4));
            Assert.That(result, Is.EqualTo(bytes.Concat(bytes).Concat(bytes).Concat(bytes)));
        }

        using (ByteWriter writer = new ByteWriter(bytes))
        {
            byte[] result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length));
            Assert.That(result, Is.EqualTo(bytes));

            writer.WriteString(encoding, value)
                .WriteString(encoding, value)
                .WriteString(encoding, value);

            result = writer.GetBytes();

            Assert.That(result.Length, Is.EqualTo(bytes.Length * 4));
            Assert.That(result, Is.EqualTo(bytes.Concat(bytes).Concat(bytes).Concat(bytes)));
        }
    }
}
