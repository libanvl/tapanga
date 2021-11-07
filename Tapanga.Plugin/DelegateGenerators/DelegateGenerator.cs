namespace Tapanga.Plugin;

/// <summary>
/// Base for a synchronous delegate generator.
/// </summary>
/// <typeparam name="T">The type of the Parameter passed to the delegate.</typeparam>
public abstract class DelegateGenerator<T> : DelegateGeneratorBase<T, int>
{
    /// <inheritdoc />
    public override bool IsAsyncDelegate => false;

    /// <summary>
    /// Validates that <paramref name="value"/> is not null, or throws.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    protected static U NotNullOrThrow<U>(U? value)
    {
        if (value is null)
        {
            throw new ArgumentException($"An expected value was not provided");
        }

        return value;
    }
}
