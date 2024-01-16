using Sernager.Core.Managers;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Sernager.Core.Utils;

internal sealed class ByteReader : IDisposable
{
    private byte[] mBytes;
    private Encoding mEncoding;
    internal int Position { get; private set; } = 0;
    internal int Length => mBytes.Length;

    internal ByteReader(string path)
    {
        using (StreamReader reader = new StreamReader(path))
        {
            mEncoding = reader.CurrentEncoding;
        }

        mBytes = File.ReadAllBytes(path);
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
            ExceptionManager.ThrowFail<ObjectDisposedException>(nameof(ByteReader));
            return Array.Empty<byte>();
        }

        if (length <= 0)
        {
            ExceptionManager.ThrowFail<ArgumentException>("Length must be greater than 0.");
            return Array.Empty<byte>();
        }

        if (Position + length > mBytes.Length)
        {
            ExceptionManager.ThrowFail<IndexOutOfRangeException>($"Position: {Position}, Length: {length}");
            return Array.Empty<byte>();
        }

        byte[] bytes = new byte[length];

        Array.Copy(mBytes, Position, bytes, 0, length);

        Position += length;

        return bytes;
    }

    internal bool TryReadBytes(int length, [NotNullWhen(true)] out byte[]? bytes)
    {
        if (mBytes == null)
        {
            bytes = null;
            return false;
        }

        if (length <= 0)
        {
            bytes = null;
            return false;
        }

        if (Position + length > mBytes.Length)
        {
            bytes = null;
            return false;
        }

        bytes = new byte[length];

        Array.Copy(mBytes, Position, bytes, 0, length);

        Position += length;

        return true;
    }

    internal bool TryReadInt32([NotNullWhen(true)] out int value)
    {
        byte[]? bytes;

        if (!TryReadBytes(sizeof(int), out bytes))
        {
            value = -1;
            return false;
        }

        value = BitConverter.ToInt32(bytes, 0);
        return true;
    }

    internal bool TryReadString(int length, [NotNullWhen(true)] out string? value)
    {
        byte[]? bytes;

        if (!TryReadBytes(length, out bytes))
        {
            value = null;
            return false;
        }

        value = mEncoding.GetString(bytes);
        return true;
    }

    internal bool TryReadString(Encoding encoding, int length, [NotNullWhen(true)] out string? value)
    {
        byte[]? bytes;

        if (!TryReadBytes(length, out bytes))
        {
            value = null;
            return false;
        }

        value = encoding.GetString(bytes);
        return true;
    }

    internal bool TrySkip(int length)
    {
        if (mBytes == null)
        {
            return false;
        }

        if (Position + length > mBytes.Length)
        {
            return false;
        }

        Position += length;
        return true;
    }
}
