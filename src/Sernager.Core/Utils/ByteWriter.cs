using System.Text;

namespace Sernager.Core.Utils;

/// <include file='docs/utils/byte_writer.xml' path='Class/Description'/> 
internal sealed class ByteWriter : IDisposable
{
    private byte[] mBytes { get; set; } = Array.Empty<byte>();

    /// <include file='docs/utils/byte_writer.xml' path='Class/Constructor[@Name="Default"]'/>
    internal ByteWriter()
    {
    }

    /// <include file='docs/utils/byte_writer.xml' path='Class/Constructor[@Name="WithBytes"]'/>
    internal ByteWriter(byte[] bytes)
    {
        mBytes = bytes;
    }

    public void Dispose()
    {
        mBytes = null!;
    }

    /// <include file='docs/utils/byte_writer.xml' path='Class/PublicMethod[@Name="WriteBytes"]'/>
    internal ByteWriter WriteBytes(byte[] bytes)
    {
        if (mBytes == null)
        {
            throw new ObjectDisposedException(nameof(ByteWriter));
        }

        byte[] newBytes = new byte[mBytes.Length + bytes.Length];

        Array.Copy(mBytes, 0, newBytes, 0, mBytes.Length);
        Array.Copy(bytes, 0, newBytes, mBytes.Length, bytes.Length);

        mBytes = newBytes;

        return this;
    }

    /// <include file='docs/utils/byte_writer.xml' path='Class/PublicMethod[@Name="WriteInt32"]'/>
    internal ByteWriter WriteInt32(int value)
    {
        if (mBytes == null)
        {
            throw new ObjectDisposedException(nameof(ByteWriter));
        }

        byte[] bytes = BitConverter.GetBytes(value);

        WriteBytes(bytes);

        return this;
    }

    /// <include file='docs/utils/byte_writer.xml' path='Class/PublicMethod[@Name="WriteString"]'/>
    internal ByteWriter WriteString(string value)
    {
        if (mBytes == null)
        {
            throw new ObjectDisposedException(nameof(ByteWriter));
        }

        byte[] bytes = Encoding.UTF8.GetBytes(value);

        WriteBytes(bytes);

        return this;
    }

    /// <include file='docs/utils/byte_writer.xml' path='Class/PublicMethod[@Name="GetBytes"]'/>
    internal byte[] GetBytes()
    {
        if (mBytes == null)
        {
            throw new ObjectDisposedException(nameof(ByteWriter));
        }

        return mBytes;
    }
}