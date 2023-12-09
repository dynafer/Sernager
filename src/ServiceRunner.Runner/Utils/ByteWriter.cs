using System.Text;

namespace ServiceRunner.Runner.Utils;

/// <include file='docs/utils/byte_writer.xml' path='Class/Description'/> 
public class ByteWriter : IDisposable
{
    private byte[] mBytes { get; set; } = Array.Empty<byte>();

    /// <include file='docs/utils/byte_writer.xml' path='Class/Constructor[@Name="Default"]'/>
    public ByteWriter()
    {
    }

    /// <include file='docs/utils/byte_writer.xml' path='Class/Constructor[@Name="WithBytes"]'/>
    public ByteWriter(byte[] bytes)
    {
        mBytes = bytes;
    }

    public void Dispose()
    {
        mBytes = null!;
    }

    /// <include file='docs/utils/byte_writer.xml' path='Class/PublicMethod[@Name="WriteBytes"]'/>
    public ByteWriter WriteBytes(byte[] bytes)
    {
        byte[] newBytes = new byte[mBytes.Length + bytes.Length];

        Array.Copy(mBytes, 0, newBytes, 0, mBytes.Length);
        Array.Copy(bytes, 0, newBytes, mBytes.Length, bytes.Length);

        mBytes = newBytes;

        return this;
    }

    /// <include file='docs/utils/byte_writer.xml' path='Class/PublicMethod[@Name="WriteInt32"]'/>
    public ByteWriter WriteInt32(int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);

        WriteBytes(bytes);

        return this;
    }

    /// <include file='docs/utils/byte_writer.xml' path='Class/PublicMethod[@Name="WriteString"]'/>
    public ByteWriter WriteString(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);

        WriteBytes(bytes);

        return this;
    }

    /// <include file='docs/utils/byte_writer.xml' path='Class/PublicMethod[@Name="GetBytes"]'/>
    public byte[] GetBytes()
    {
        return mBytes;
    }
}
