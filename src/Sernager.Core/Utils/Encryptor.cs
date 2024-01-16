using Sernager.Core.Managers;
using System.Security.Cryptography;
using System.Text;

namespace Sernager.Core.Utils;

internal static class Encryptor
{
    internal const int KEY_SIZE = 32;
    internal const int IV_SIZE = 16;

    internal static byte[] Encrypt(string value, string key, string iv)
    {
        if (value == null)
        {
            ExceptionManager.ThrowFail<ArgumentNullException>(nameof(value));
            return Array.Empty<byte>();
        }

        if (key == null)
        {
            ExceptionManager.ThrowFail<ArgumentNullException>(nameof(key));
            return Array.Empty<byte>();
        }

        if (iv == null)
        {
            ExceptionManager.ThrowFail<ArgumentNullException>(nameof(iv));
            return Array.Empty<byte>();
        }

        if (key.Length != KEY_SIZE)
        {
            ExceptionManager.ThrowFail<ArgumentException>("Invalid key size.");
            return Array.Empty<byte>();
        }

        if (iv.Length != IV_SIZE)
        {
            ExceptionManager.ThrowFail<ArgumentException>("Invalid iv size.");
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

    internal static string Decrypt(byte[] value, byte[] key, byte[] iv)
    {
        if (value == null)
        {
            ExceptionManager.ThrowFail<ArgumentNullException>(nameof(value));
            return string.Empty;
        }

        if (key == null)
        {
            ExceptionManager.ThrowFail<ArgumentNullException>(nameof(key));
            return string.Empty;
        }

        if (iv == null)
        {
            ExceptionManager.ThrowFail<ArgumentNullException>(nameof(iv));
            return string.Empty;
        }

        if (key.Length != KEY_SIZE)
        {
            ExceptionManager.ThrowFail<ArgumentException>("Invalid key size.");
            return string.Empty;
        }

        if (iv.Length != IV_SIZE)
        {
            ExceptionManager.ThrowFail<ArgumentException>("Invalid iv size.");
            return string.Empty;
        }

        string text = "";

        using (Aes aes = Aes.Create())
        {
            aes.Padding = PaddingMode.Zeros;
            aes.Key = key;
            aes.IV = iv;

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
