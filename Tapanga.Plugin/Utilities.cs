namespace Tapanga;

public static class Utilities
{
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

    public static bool IsQuoted(string v) => (v.First() == '"' || v.First() == '\'') && v.First() == v.Last();

    public static string QuoteFormat(string v)
    {
        if (IsQuoted(v))
        {
            return v;
        }

        if (v.Any(c => char.IsWhiteSpace(c)))
        {
            return $"\"{v}\"";
        }

        return v;
    }
}
