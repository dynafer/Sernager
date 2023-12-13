using Sernager.Core.Managers;

namespace Sernager.Core.Utils;

internal static class Randomizer
{
    private readonly static string ALL_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    internal static string GenerateRandomString(int length)
    {
        if (length < 0)
        {
            ErrorManager.ThrowFail<ArgumentException>("Length cannot be negative");
            return string.Empty;
        }

        return new string(Enumerable.Repeat(ALL_CHARS, length)
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }

    internal static string GenerateRandomString(int minLength, int maxLength)
    {
        if (minLength < 0 || maxLength < 0 || minLength > maxLength)
        {
            ErrorManager.ThrowFail<ArgumentException>("Length cannot be negative or min length cannot be greater than max length");
            return string.Empty;
        }

        int length = Random.Shared.Next(minLength, maxLength + 1);

        return GenerateRandomString(length);
    }
}
