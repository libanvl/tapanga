namespace Tapanga;

public static class Utilities
{
    public static string GetShortRandomId(int bytes = 4)
    {
        if (bytes > 16 || bytes < 1)
        {
            throw new ArgumentException(
                $"{nameof(bytes)} must be greater than 0 and less than 16",
                nameof(bytes));
        }

        return Convert.ToHexString(Guid.NewGuid().ToByteArray(), 0, bytes).ToLowerInvariant();
    }

    public static string QuoteFormat(string v)
    {
        if ((v.First() == '"' || v.First() == '\'') && v.First() == v.Last())
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
