namespace Ftp.Identity.KeyVault.Extensions;

public static class StringExtensions
{
    public static string ToUpperFirstLetter(this string source)
    {
        if (string.IsNullOrEmpty(source))
            return string.Empty;

        source = source.ToLowerInvariant();

        char[] letters = source.ToCharArray();

        letters[0] = char.ToUpper(letters[0]);

        return new string(letters);
    }
}