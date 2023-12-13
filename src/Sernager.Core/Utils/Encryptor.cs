using Sernager.Core.Managers;
using System.Security.Cryptography;
using System.Text;

namespace Sernager.Core.Utils;

internal static class Encryptor
{
    internal static byte[] Encrypt(string value, string key, string iv)
    {
        if (value == null)
        {
            ErrorManager.ThrowFail<ArgumentNullException>(nameof(value));
            return Array.Empty<byte>();
        }

        byte[] encrypted;

        using (Aes aes = Aes.Create())
        {
            aes.Padding = PaddingMode.Zeros;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(value);
                }

                encrypted = memoryStream.ToArray();
            }
        }

        return encrypted;
    }

    internal static string Decrypt(byte[] value, string key, string iv)
    {
        string text = "";

        using (Aes aes = Aes.Create())
        {
            aes.Padding = PaddingMode.Zeros;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(value))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (StreamReader streamReader = new StreamReader(cryptoStream))
                {
                    text = streamReader.ReadToEnd();
                }
            }
        }

        return text.Replace("\0", "").Trim();
    }
}
