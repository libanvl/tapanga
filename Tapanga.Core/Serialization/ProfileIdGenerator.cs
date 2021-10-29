namespace Tapanga.Core.Serialization;

internal class ProfileIdGenerator
{
    private readonly Random _random = new Random();
    private readonly int _bytes;

    public ProfileIdGenerator(int bytes = 4)
    {
        if (bytes < 1)
        {
            throw new ArgumentException(
                $"{nameof(bytes)} must be greater than 0",
                nameof(bytes));
        }

        _bytes = bytes;
    }

    public string GenerateId()
    {
        var buffer = new byte[_bytes];
        _random.NextBytes(buffer);
        return Convert.ToHexString(buffer).ToLowerInvariant();
    }
}
