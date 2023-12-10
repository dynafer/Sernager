namespace ServiceRunner.Runner.Utils;

public static class Randomizer
{
    private readonly static string ALL_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    /// <include file='docs/utils/randomizer.xml' path='Class/PublicStaticMethod[@Name="GenerateRandomString"][@Type="FixedLength"]'/>
    public static string GenerateRandomString(int length)
    {
        if (length < 0)
        {
            throw new ArgumentException("Length cannot be negative");
        }

        return new string(Enumerable.Repeat(ALL_CHARS, length)
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }

    /// <include file='docs/utils/randomizer.xml' path='Class/PublicStaticMethod[@Name="GenerateRandomString"][@Type="RangeLength"]'/>
    public static string GenerateRandomString(int minLength, int maxLength)
    {
        if (minLength < 0 || maxLength < 0 || minLength > maxLength)
        {
            throw new ArgumentException("Length cannot be negative or min length cannot be greater than max length");
        }

        int length = Random.Shared.Next(minLength, maxLength + 1);

        return GenerateRandomString(length);
    }
}
