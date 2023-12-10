using System.Security.Cryptography;
using System.Text;

namespace ServiceRunner.Runner.Utils;

internal static class Encryptor
{
    /// <include file='docs/utils/encryptor.xml' path='Class/InternalStaticMethod[@Name="Encrypt"]'/>
    internal static byte[] Encrypt(string value, string key, string iv)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        byte[] encrypted;

        using (Aes aes = Aes.Create())
        {
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

    /// <include file='docs/utils/encryptor.xml' path='Class/InternalStaticMethod[@Name="Decrypt"]'/>
    internal static string Decrypt(byte[] value, string key, string iv)
    {
        string text = "";

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(value))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            using (StreamReader streamReader = new StreamReader(cryptoStream))
            {
                text = streamReader.ReadToEnd();
            }
        }

        return text;
    }
}
