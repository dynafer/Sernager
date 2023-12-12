using Sernager.Runner.Utils;
using System.Security.Cryptography;

namespace Sernager.Runner.Tests.Units.Utils;

public class EncryptorTest
{
    private string mKey { get; set; }
    private string mIV { get; set; }

    [SetUp]
    public void Setup()
    {
        mKey = Randomizer.GenerateRandomString(32);
        mIV = Randomizer.GenerateRandomString(16);
    }

    [Test]
    public void EncryptAndDecrypt_ShouldReturnOriginalValue()
    {
        string originalValue = "Test String";

        byte[] encryptedValue = Encryptor.Encrypt(originalValue, mKey, mIV);
        string decryptedValue = Encryptor.Decrypt(encryptedValue, mKey, mIV);

        Assert.That(decryptedValue, Is.EqualTo(originalValue));
    }

    [Test]
    public void Encrypt_WithSpecialCharacters_ShouldReturnCorrectValue()
    {
        string originalValue = "!@#$%^&*()_+{}|:\"<>?";

        byte[] encryptedValue = Encryptor.Encrypt(originalValue, mKey, mIV);
        string decryptedValue = Encryptor.Decrypt(encryptedValue, mKey, mIV);

        Assert.That(decryptedValue, Is.EqualTo(originalValue));
    }

    [Test]
    public void Encrypt_WithNullValue_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => Encryptor.Encrypt(null!, mKey, mIV));
    }

    [Test]
    public void Decrypt_WithNullValue_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => Encryptor.Decrypt(null!, mKey, mIV));
    }

    [Test]
    public void Encrypt_WithInvalidKey_ShouldThrowException()
    {
        Assert.Throws<CryptographicException>(() => Encryptor.Encrypt("Test String", "invalidkey", mIV));
    }

    [Test]
    public void Decrypt_WithInvalidKey_ShouldThrowException()
    {
        byte[] encryptedValue = Encryptor.Encrypt("Test String", mKey, mIV);

        Assert.Throws<CryptographicException>(() => Encryptor.Decrypt(encryptedValue, "invalidkey", mIV));
    }

    [Test]
    public void Encrypt_WithInvalidIv_ShouldThrowException()
    {
        Assert.Throws<CryptographicException>(() => Encryptor.Encrypt("Test String", mKey, "invalidiv"));
    }

    [Test]
    public void Decrypt_WithInvalidIv_ShouldThrowException()
    {
        byte[] encryptedValue = Encryptor.Encrypt("Test String", mKey, mIV);

        Assert.Throws<CryptographicException>(() => Encryptor.Decrypt(encryptedValue, mKey, "invalidiv"));
    }

    [Test]
    public void Encrypt_WithDifferentKeys_ShouldReturnDifferentValues()
    {
        string value = "Test String";

        byte[] encryptedValue1 = Encryptor.Encrypt(value, mKey, mIV);
        byte[] encryptedValue2 = Encryptor.Encrypt(value, Randomizer.GenerateRandomString(32), mIV);

        Assert.That(encryptedValue1, Is.Not.EqualTo(encryptedValue2));
    }

    [Test]
    public void Decrypt_WithWrongKey_ShouldThrowException()
    {
        string value = "Test String";

        byte[] encryptedValue = Encryptor.Encrypt(value, mKey, mIV);

        Assert.Throws<CryptographicException>(() => Encryptor.Decrypt(encryptedValue, Randomizer.GenerateRandomString(60), mIV));
    }

    [Test]
    public void Encrypt_WithDifferentIvs_ShouldReturnDifferentValues()
    {
        string value = "Test String";

        byte[] encryptedValue1 = Encryptor.Encrypt(value, mKey, mIV);
        byte[] encryptedValue2 = Encryptor.Encrypt(value, mKey, Randomizer.GenerateRandomString(16));

        Assert.That(encryptedValue1, Is.Not.EqualTo(encryptedValue2));
    }

    [Test]
    public void Decrypt_WithWrongIv_ShouldThrowException()
    {
        string value = "Test String";

        byte[] encryptedValue = Encryptor.Encrypt(value, mKey, mIV);

        Assert.Throws<CryptographicException>(() => Encryptor.Decrypt(encryptedValue, mKey, Randomizer.GenerateRandomString(50)));
    }
}
