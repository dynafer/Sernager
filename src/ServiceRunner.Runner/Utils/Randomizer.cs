namespace ServiceRunner.Runner.Utils;

public static class Randomizer
{
    private readonly static string ALL_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    /// <include file='docs/utils/randomizer.xml' path='Class/PublicStaticMethod[@Name="GenerateRandomString"][@Type="FixedLength"]'/>
    public static string GenerateRandomString(int length)
    {
        return new string(Enumerable.Repeat(ALL_CHARS, length)
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }

    /// <include file='docs/utils/randomizer.xml' path='Class/PublicStaticMethod[@Name="GenerateRandomString"][@Type="RangeLength"]'/>
    public static string GenerateRandomString(int minLength, int maxLength)
    {
        int length = Random.Shared.Next(minLength, maxLength + 1);

        return GenerateRandomString(length);
    }
}
