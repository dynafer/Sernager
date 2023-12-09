using System.Text;

namespace ServiceRunner.Runner.Utils;

/// <include file='docs/utils/byte_reader.xml' path='Class/Description'/> 
internal class ByteReader : IDisposable
{
    private byte[] mBytes { get; set; }
    internal int Position { get; private set; } = 0;
    internal int Length => mBytes.Length;

    /// <include file='docs/utils/byte_reader.xml' path='Class/Constructor'/>
    internal ByteReader(byte[] bytes)
    {
        mBytes = bytes;
    }

    public void Dispose()
    {
        mBytes = null!;
        Position = 0;
    }

    /// <include file='docs/utils/byte_reader.xml' path='Class/PublicMethod[@Name="ReadBytes"]'/>
    internal byte[] ReadBytes(int length)
    {
        byte[] bytes = new byte[length];

        Array.Copy(mBytes, Position, bytes, 0, length);

        Position += length;

        return bytes;
    }

    /// <include file='docs/utils/byte_reader.xml' path='Class/PublicMethod[@Name="ReadInt32"]'/>
    internal int ReadInt32()
    {
        byte[] bytes = ReadBytes(sizeof(int));

        return BitConverter.ToInt32(bytes, 0);
    }

    /// <include file='docs/utils/byte_reader.xml' path='Class/PublicMethod[@Name="ReadString"]'/>
    internal string ReadString(int length)
    {
        byte[] bytes = ReadBytes(length);

        return Encoding.UTF8.GetString(bytes);
    }
}
