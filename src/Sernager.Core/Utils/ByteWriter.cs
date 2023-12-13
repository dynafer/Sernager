using Sernager.Core.Managers;
using System.Text;

namespace Sernager.Core.Utils;

internal sealed class ByteWriter : IDisposable
{
    private byte[] mBytes { get; set; } = Array.Empty<byte>();

    internal ByteWriter()
    {
    }

    internal ByteWriter(byte[] bytes)
    {
        mBytes = bytes;
    }

    public void Dispose()
    {
        mBytes = null!;
    }

    internal ByteWriter WriteBytes(byte[] bytes)
    {
        if (mBytes == null)
        {
            ErrorManager.ThrowFail<ObjectDisposedException>(nameof(ByteWriter));
            return this;
        }

        byte[] newBytes = new byte[mBytes.Length + bytes.Length];

        Array.Copy(mBytes, 0, newBytes, 0, mBytes.Length);
        Array.Copy(bytes, 0, newBytes, mBytes.Length, bytes.Length);

        mBytes = newBytes;

        return this;
    }

    internal ByteWriter WriteInt32(int value)
    {
        if (mBytes == null)
        {
            ErrorManager.ThrowFail<ObjectDisposedException>(nameof(ByteWriter));
            return this;
        }

        byte[] bytes = BitConverter.GetBytes(value);

        WriteBytes(bytes);

        return this;
    }

    internal ByteWriter WriteString(string value)
    {
        if (mBytes == null)
        {
            ErrorManager.ThrowFail<ObjectDisposedException>(nameof(ByteWriter));
            return this;
        }

        byte[] bytes = Encoding.UTF8.GetBytes(value);

        WriteBytes(bytes);

        return this;
    }

    internal byte[] GetBytes()
    {
        if (mBytes == null)
        {
            ErrorManager.ThrowFail<ObjectDisposedException>(nameof(ByteWriter));
            return Array.Empty<byte>();
        }

        return mBytes;
    }
}
