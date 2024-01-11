using Sernager.Core.Tests.Fixtures;
using Sernager.Core.Utils;
using System.Text;

namespace Sernager.Core.Tests.Units.Utils;

public class EncryptorFailureTests : FailureFixture
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
        string key = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE);
        string iv = Randomizer.GenerateRandomString(Encryptor.IV_SIZE);

        TestNoneLevel(() => Encryptor.Decrypt(null!, key, iv), Is.Empty);
        TestExceptionLevel<ArgumentNullException>(() => Encryptor.Decrypt(null!, key, iv));
    }

    [Test]
    public void Decrypt_ShouldThrow_WhenPassedNullKey()
    {
        byte[] value = Encoding.UTF8.GetBytes("Hello, World!");
        string iv = Randomizer.GenerateRandomString(Encryptor.IV_SIZE);

        TestNoneLevel(() => Encryptor.Decrypt(value, null!, iv), Is.Empty);
        TestExceptionLevel<ArgumentNullException>(() => Encryptor.Decrypt(value, null!, iv));
    }

    [Test]
    public void Decrypt_ShouldThrow_WhenPassedNullIv()
    {
        byte[] value = Encoding.UTF8.GetBytes("Hello, World!");
        string key = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE);

        TestNoneLevel(() => Encryptor.Decrypt(value, key, null!), Is.Empty);
        TestExceptionLevel<ArgumentNullException>(() => Encryptor.Decrypt(value, key, null!));
    }

    [Test]
    public void Decrypt_ShouldThrow_WhenPassedInvalidKeySize()
    {
        byte[] value = Encoding.UTF8.GetBytes("Hello, World!");
        string key = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE - 1);
        string iv = Randomizer.GenerateRandomString(Encryptor.IV_SIZE);

        TestNoneLevel(() => Encryptor.Decrypt(value, key, iv), Is.Empty);
        TestExceptionLevel<ArgumentException>(() => Encryptor.Decrypt(value, key, iv));
    }

    [Test]
    public void Decrypt_ShouldThrow_WhenPassedInvalidIvSize()
    {
        byte[] value = Encoding.UTF8.GetBytes("Hello, World!");
        string key = Randomizer.GenerateRandomString(Encryptor.KEY_SIZE);
        string iv = Randomizer.GenerateRandomString(Encryptor.IV_SIZE - 1);

        TestNoneLevel(() => Encryptor.Decrypt(value, key, iv), Is.Empty);
        TestExceptionLevel<ArgumentException>(() => Encryptor.Decrypt(value, key, iv));
    }
}
