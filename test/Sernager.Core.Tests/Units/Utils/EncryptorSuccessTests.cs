using Sernager.Core.Utils;
using System.Text;

namespace Sernager.Core.Tests.Units.Utils;

public class EncryptorSuccessTests
{
    [Test]
    public void Encrypt_And_Decrypt_ShouldWorkProperly()
    {
        string cypherText = "Hello World!";
        string key = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE);
        string iv = Randomizer.GenerateRandomString(Encryptor.IV_SIZE);

        byte[] encryptedBytes = Encryptor.Encrypt(cypherText, key, iv);

        Assert.That(encryptedBytes, Is.Not.Null);
        Assert.That(encryptedBytes, Is.Not.Empty);
        Assert.That(encryptedBytes, Is.Not.EqualTo(Encoding.Default.GetBytes(cypherText)));

        string decryptedText = Encryptor.Decrypt(encryptedBytes, key, iv);

        Assert.That(decryptedText, Is.Not.Null);
        Assert.That(decryptedText, Is.Not.Empty);
        Assert.That(decryptedText, Is.EqualTo(cypherText));

        cypherText = "{ \"Hello\": \"World\" }";

        encryptedBytes = Encryptor.Encrypt(cypherText, key, iv);

        Assert.That(encryptedBytes, Is.Not.Null);
        Assert.That(encryptedBytes, Is.Not.Empty);
        Assert.That(encryptedBytes, Is.Not.EqualTo(Encoding.Default.GetBytes(cypherText)));

        decryptedText = Encryptor.Decrypt(encryptedBytes, key, iv);

        Assert.That(decryptedText, Is.Not.Null);
        Assert.That(decryptedText, Is.Not.Empty);
        Assert.That(decryptedText, Is.EqualTo(cypherText));
    }
}
