using Sernager.Core.Utils;
using System.Text;

namespace Sernager.Core.Tests.Units.Utils;

internal sealed class EncryptorFailureTests : FailureFixture
{
    [Test]
    public void Encrypt_ShouldThrow_WhenPassedNullValue()
    {
        string key = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE);
        string iv = Randomizer.GenerateRandomString(Encryptor.IV_SIZE);

        TestNoneLevel(() => Encryptor.Encrypt(null!, key, iv), Is.Empty);
        TestExceptionLevel<ArgumentNullException>(() => Encryptor.Encrypt(null!, key, iv));
    }

    [Test]
    public void Encrypt_ShouldThrow_WhenPassedNullKey()
    {
        string value = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE);
        string iv = Randomizer.GenerateRandomString(Encryptor.IV_SIZE);

        TestNoneLevel(() => Encryptor.Encrypt(value, null!, iv), Is.Empty);
        TestExceptionLevel<ArgumentNullException>(() => Encryptor.Encrypt(value, null!, iv));
    }

    [Test]
    public void Encrypt_ShouldThrow_WhenPassedNullIv()
    {
        string value = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE);
        string key = Randomizer.GenerateRandomString(Encryptor.IV_SIZE);

        TestNoneLevel(() => Encryptor.Encrypt(value, key, null!), Is.Empty);
        TestExceptionLevel<ArgumentNullException>(() => Encryptor.Encrypt(value, key, null!));
    }

    [Test]
    public void Encrypt_ShouldThrow_WhenPassedInvalidKeySize()
    {
        string value = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE);
        string key = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE - 1);
        string iv = Randomizer.GenerateRandomString(Encryptor.IV_SIZE);

        TestNoneLevel(() => Encryptor.Encrypt(value, key, iv), Is.Empty);
        TestExceptionLevel<ArgumentException>(() => Encryptor.Encrypt(value, key, iv));
    }

    [Test]
    public void Encrypt_ShouldThrow_WhenPassedInvalidIvSize()
    {
        string value = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE);
        string key = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE);
        string iv = Randomizer.GenerateRandomString(Encryptor.IV_SIZE - 1);

        TestNoneLevel(() => Encryptor.Encrypt(value, key, iv), Is.Empty);
        TestExceptionLevel<ArgumentException>(() => Encryptor.Encrypt(value, key, iv));
    }

    [Test]
    public void Decrypt_ShouldThrow_WhenPassedNullValue()
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(Randomizer.GenerateRandomString(Encryptor.KEY_SIZE));
        byte[] ivBytes = Encoding.UTF8.GetBytes(Randomizer.GenerateRandomString(Encryptor.IV_SIZE));

        TestNoneLevel(() => Encryptor.Decrypt(null!, keyBytes, ivBytes), Is.Empty);
        TestExceptionLevel<ArgumentNullException>(() => Encryptor.Decrypt(null!, keyBytes, ivBytes));
    }

    [Test]
    public void Decrypt_ShouldThrow_WhenPassedNullKey()
    {
        byte[] value = Encoding.Default.GetBytes("Hello, World!");
        byte[] ivBytes = Encoding.UTF8.GetBytes(Randomizer.GenerateRandomString(Encryptor.IV_SIZE));

        TestNoneLevel(() => Encryptor.Decrypt(value, null!, ivBytes), Is.Empty);
        TestExceptionLevel<ArgumentNullException>(() => Encryptor.Decrypt(value, null!, ivBytes));
    }

    [Test]
    public void Decrypt_ShouldThrow_WhenPassedNullIv()
    {
        byte[] value = Encoding.Default.GetBytes("Hello, World!");
        byte[] keyBytes = Encoding.UTF8.GetBytes(Randomizer.GenerateRandomString(Encryptor.KEY_SIZE));

        TestNoneLevel(() => Encryptor.Decrypt(value, keyBytes, null!), Is.Empty);
        TestExceptionLevel<ArgumentNullException>(() => Encryptor.Decrypt(value, keyBytes, null!));
    }

    [Test]
    public void Decrypt_ShouldThrow_WhenPassedInvalidKeySize()
    {
        byte[] value = Encoding.Default.GetBytes("Hello, World!");
        byte[] keyBytes = Encoding.UTF8.GetBytes(Randomizer.GenerateRandomString(Encryptor.KEY_SIZE - 1));
        byte[] ivBytes = Encoding.UTF8.GetBytes(Randomizer.GenerateRandomString(Encryptor.IV_SIZE));

        TestNoneLevel(() => Encryptor.Decrypt(value, keyBytes, ivBytes), Is.Empty);
        TestExceptionLevel<ArgumentException>(() => Encryptor.Decrypt(value, keyBytes, ivBytes));
    }

    [Test]
    public void Decrypt_ShouldThrow_WhenPassedInvalidIvSize()
    {
        byte[] value = Encoding.Default.GetBytes("Hello, World!");
        byte[] keyBytes = Encoding.UTF8.GetBytes(Randomizer.GenerateRandomString(Encryptor.KEY_SIZE));
        byte[] ivBytes = Encoding.UTF8.GetBytes(Randomizer.GenerateRandomString(Encryptor.IV_SIZE - 1));

        TestNoneLevel(() => Encryptor.Decrypt(value, keyBytes, ivBytes), Is.Empty);
        TestExceptionLevel<ArgumentException>(() => Encryptor.Decrypt(value, keyBytes, ivBytes));
    }
}
