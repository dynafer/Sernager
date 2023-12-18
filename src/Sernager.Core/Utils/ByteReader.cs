using Sernager.Core.Managers;
using System.Text;

namespace Sernager.Core.Utils;

internal sealed class ByteReader : IDisposable
{
    private byte[] mBytes;
    internal int Position { get; private set; } = 0;
    internal int Length => mBytes.Length;

    internal ByteReader(byte[] bytes)
    {
        mBytes = bytes;
    }

    public void Dispose()
    {
        mBytes = null!;
        Position = 0;
    }

    internal byte[] ReadBytes(int length)
    {
        if (mBytes == null)
        {
            ErrorManager.ThrowFail<ObjectDisposedException>(nameof(ByteReader));
            return Array.Empty<byte>();
        }

        if (Position + length > mBytes.Length)
        {
            ErrorManager.ThrowFail<IndexOutOfRangeException>(string.Format("Position: {0}, Length: {1}", Position, length));
            return Array.Empty<byte>();
        }

        byte[] bytes = new byte[length];

        Array.Copy(mBytes, Position, bytes, 0, length);

        Position += length;

        return bytes;
    }

    internal int ReadInt32()
    {
        if (mBytes == null)
        {
            ErrorManager.ThrowFail<ObjectDisposedException>(nameof(ByteReader));
            return 0;
        }

        byte[] bytes = ReadBytes(sizeof(int));

        return BitConverter.ToInt32(bytes, 0);
    }

    internal string ReadString(int length)
    {
        if (mBytes == null)
        {
            ErrorManager.ThrowFail<ObjectDisposedException>(nameof(ByteReader));
            return string.Empty;
        }

        byte[] bytes = ReadBytes(length);

        return Encoding.UTF8.GetString(bytes);
    }

    internal void Skip(int length)
    {
        if (mBytes == null)
        {
            ErrorManager.ThrowFail<ObjectDisposedException>(nameof(ByteReader));
            return;
        }

        if (Position + length > mBytes.Length)
        {
            ErrorManager.ThrowFail<IndexOutOfRangeException>(string.Format("Position: {0}, Length: {1}", Position, length));
            return;
        }

        Position += length;
    }
}
