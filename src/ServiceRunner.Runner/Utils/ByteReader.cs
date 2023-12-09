using System.Text;

namespace ServiceRunner.Runner.Utils;

public class ByteReader
{
    private readonly byte[] mBytes;
    public int Position { get; private set; } = 0;
    public int Length => mBytes.Length;

    public ByteReader(byte[] bytes)
    {
        mBytes = bytes;
    }

    public byte[] ReadBytes(int length)
    {
        byte[] bytes = new byte[length];

        Array.Copy(mBytes, Position, bytes, 0, length);

        Position += length;

        return bytes;
    }

    public int ReadInt32()
    {
        byte[] bytes = ReadBytes(sizeof(int));

        return BitConverter.ToInt32(bytes, 0);
    }

    public string ReadString(int length)
    {
        byte[] bytes = ReadBytes(length);

        return Encoding.UTF8.GetString(bytes);
    }
}
