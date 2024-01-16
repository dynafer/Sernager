using System.Text;

namespace Sernager.Unit.Extensions;

public static class EncodingExtension
{
    public static string GetEncodingName(this Encoding encoding)
    {
        switch (encoding.CodePage)
        {
            case 65001: // Encoding.UTF8.CodePage
                return "UTF8";
            case 1200: // Encoding.Unicode.CodePage
                return "Unicode";
            case 1201: // Encoding.BigEndianUnicode.CodePage
                return "BigEndianUnicode";
            case 12000: // Encoding.UTF32.CodePage
                return "UTF32";
            case 20127: // Encoding.ASCII.CodePage
                return "ASCII";
            default:
                return "Unknown";
        }
    }
}
