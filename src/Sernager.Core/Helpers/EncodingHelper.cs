using System.Text;

namespace Sernager.Core.Helpers;

public static class EncodingHelper
{
    public static Encoding Predict(byte[] bytes)
    {
        if (bytes.Length < 4)
        {
            return Encoding.Default;
        }

        if (bytes[0] == 0x2b && bytes[1] == 0x2f && bytes[2] == 0x76)
        {
#pragma warning disable SYSLIB0001
            return Encoding.UTF7;
#pragma warning restore SYSLIB0001
        }
        else if (bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf)
        {
            return Encoding.UTF8;
        }
        else if (bytes[0] == 0xff && bytes[1] == 0xfe)
        {
            return Encoding.Unicode;
        }
        else if (bytes[0] == 0xfe && bytes[1] == 0xff)
        {
            return Encoding.BigEndianUnicode;
        }
        else if (bytes[0] == 0 && bytes[1] == 0 && bytes[2] == 0xfe && bytes[3] == 0xff)
        {
            return Encoding.UTF32;
        }
        else
        {
            return Encoding.Default;
        }
    }

    public static string GetString(byte[] bytes)
    {
        Encoding encoding = Predict(bytes);

        return encoding.GetString(bytes);
    }
}
