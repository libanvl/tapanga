namespace Tapanga;

public static class Utilities
{
    /// <summary>
    /// Creates a short pseudo-random identifier as a hex string.
    /// </summary>
    /// <remarks>The sum of <paramref name="bytes"/> and <paramref name="offset"/> must be less than 16.</remarks>
    /// <param name="bytes">The number of bytes to use to create the id.</param>
    /// <param name="offset">The offset of bytes from the pseudo-random number used to generate the id.</param>
    /// <returns>A string of hex digits.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static string GetShortRandomId(int bytes = 4, int offset = 0)
    {
        if (bytes > 16 || bytes < 1)
        {
            throw new ArgumentException(
                $"{nameof(bytes)} must be greater than 0 and less than 16",
                nameof(bytes));
        }

        if (offset + bytes > 16)
        {
            throw new ArgumentException(
                $"total of {nameof(bytes)} + {nameof(offset)} must be less than 16",
                nameof(offset));
        }

        Guid guid = Guid.NewGuid();
        return Convert.ToHexString(guid.ToByteArray(), offset, bytes).ToLowerInvariant();
    }

    /// <summary>
    /// Whether a string is surrounded by single or double quotes.
    /// </summary>
    /// <returns><c>true</c> if the string is quoted, <c>false</c> otherwise.</returns>
    public static bool IsQuoted(string v) => (v.First() == '"' || v.First() == '\'') && v.First() == v.Last();

    /// <summary>
    /// Returns a new string surrounded by double-quotes iff the <paramref name="value"/> contains whitespace and is not already quoted.
    /// </summary>
    public static string QuoteFormat(string value)
    {
        if (IsQuoted(value))
        {
            return value;
        }

        if (value.Any(c => char.IsWhiteSpace(c)))
        {
            return $"\"{value}\"";
        }

        return value;
    }
}
