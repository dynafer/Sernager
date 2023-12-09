using System.Text;

namespace ServiceRunner.Runner.Utils;

/// <include file='docs/utils/byte_reader.xml' path='Class/Description'/> 
public class ByteReader : IDisposable
{
    private byte[] mBytes { get; set; }
    public int Position { get; private set; } = 0;
    public int Length => mBytes.Length;

    /// <include file='docs/utils/byte_reader.xml' path='Class/Constructor'/>
    public ByteReader(byte[] bytes)
    {
        mBytes = bytes;
    }

    public void Dispose()
    {
        mBytes = null!;
        Position = 0;
    }

    /// <include file='docs/utils/byte_reader.xml' path='Class/PublicMethod[@Name="ReadBytes"]'/>
    public byte[] ReadBytes(int length)
    {
        byte[] bytes = new byte[length];

        Array.Copy(mBytes, Position, bytes, 0, length);

        Position += length;

        return bytes;
    }

    /// <include file='docs/utils/byte_reader.xml' path='Class/PublicMethod[@Name="ReadInt32"]'/>
    public int ReadInt32()
    {
        byte[] bytes = ReadBytes(sizeof(int));

        return BitConverter.ToInt32(bytes, 0);
    }

    /// <include file='docs/utils/byte_reader.xml' path='Class/PublicMethod[@Name="ReadString"]'/>
    public string ReadString(int length)
    {
        byte[] bytes = ReadBytes(length);

        return Encoding.UTF8.GetString(bytes);
    }
}
