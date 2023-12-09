using System.Text;

namespace ServiceRunner.Runner.Utils;

public class ByteWriter
{
    private byte[] mBytes { get; set; } = Array.Empty<byte>();

    public ByteWriter()
    {
    }

    public ByteWriter WriteBytes(byte[] bytes)
    {
        byte[] newBytes = new byte[mBytes.Length + bytes.Length];

        Array.Copy(mBytes, 0, newBytes, 0, mBytes.Length);
        Array.Copy(bytes, 0, newBytes, mBytes.Length, bytes.Length);

        mBytes = newBytes;

        return this;
    }

    public ByteWriter WriteInt32(int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);

        WriteBytes(bytes);

        return this;
    }

    public ByteWriter WriteString(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);

        WriteBytes(bytes);

        return this;
    }

    public byte[] GetBytes()
    {
        return mBytes;
    }
}
