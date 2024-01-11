using Sernager.Core.Utils;
using System.Text;

namespace Sernager.Core.Tests.Units.Utils;

public class ByteReaderSuccessTests
{
    [Test]
    public void ReadBytes_ShouldReturnEmptyBytes()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];

        using (ByteReader reader = new ByteReader(bytes))
        {
            byte[] result = reader.ReadBytes(0);

            Assert.That(reader.Length, Is.EqualTo(bytes.Length));
            Assert.That(reader.Position, Is.Zero);
            Assert.That(result, Is.Empty);
        }
    }

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
    public void ReadInt32_ShouldReturnInt32()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];

        using (ByteReader reader = new ByteReader(bytes))
        {
            int result = reader.ReadInt32();

            Assert.That(reader.Length, Is.EqualTo(sizeof(int)));
            Assert.That(reader.Position, Is.EqualTo(sizeof(int)));
            Assert.That(result, Is.EqualTo(0x04030201));
        }

        int value = 20212223;
        bytes = BitConverter.GetBytes(value);

        using (ByteReader reader = new ByteReader(bytes))
        {
            int result = reader.ReadInt32();

            Assert.That(reader.Length, Is.EqualTo(sizeof(int)));
            Assert.That(reader.Position, Is.EqualTo(sizeof(int)));
            Assert.That(result, Is.EqualTo(value));
        }
    }

    [Test]
    public void ReadString_ShouldReturnEmptyString()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04];

        using (ByteReader reader = new ByteReader(bytes))
        {
            string result = reader.ReadString(0);

            Assert.That(reader.Length, Is.EqualTo(bytes.Length));
            Assert.That(reader.Position, Is.Zero);
            Assert.That(result, Is.Empty);
        }
    }

    [Test]
    public void ReadString_ShouldReturnString()
    {
        string value = "Hello, World!";
        byte[] bytes = Encoding.UTF8.GetBytes(value);

        using (ByteReader reader = new ByteReader(bytes))
        {
            string result = reader.ReadString(5);

            Assert.That(reader.Length, Is.EqualTo(bytes.Length));

            Assert.That(reader.Position, Is.EqualTo(5));
            Assert.That(result, Is.EqualTo("Hello"));

            result = reader.ReadString(8);

            Assert.That(reader.Position, Is.EqualTo(bytes.Length));
            Assert.That(result, Is.EqualTo(", World!"));
        }

        value = "{ \"Hello\": \"World!\" }";
        bytes = Encoding.UTF8.GetBytes(value);

        using (ByteReader reader = new ByteReader(bytes))
        {
            string result = reader.ReadString(2);

            Assert.That(reader.Length, Is.EqualTo(bytes.Length));

            Assert.That(reader.Position, Is.EqualTo(2));
            Assert.That(result, Is.EqualTo("{ "));

            result = reader.ReadString(9);

            Assert.That(reader.Position, Is.EqualTo(11));
            Assert.That(result, Is.EqualTo("\"Hello\": "));

            result = reader.ReadString(8);

            Assert.That(reader.Position, Is.EqualTo(19));
            Assert.That(result, Is.EqualTo("\"World!\""));

            result = reader.ReadString(2);

            Assert.That(reader.Position, Is.EqualTo(bytes.Length));
            Assert.That(result, Is.EqualTo(" }"));
        }
    }

    [Test]
    public void Skip_With_ReadBytes_ShouldReturnNextSkippedBytes()
    {
        byte[] bytes = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06];

        using (ByteReader reader = new ByteReader(bytes))
        {
            byte[] result = reader.ReadBytes(2);

            Assert.That(reader.Length, Is.EqualTo(bytes.Length));

            Assert.That(reader.Position, Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(new byte[] { 0x01, 0x02 }));

            reader.Skip(2);

            Assert.That(reader.Position, Is.EqualTo(4));

            result = reader.ReadBytes(2);

            Assert.That(reader.Position, Is.EqualTo(6));
            Assert.That(result, Is.EqualTo(new byte[] { 0x05, 0x06 }));
        }
    }
}
