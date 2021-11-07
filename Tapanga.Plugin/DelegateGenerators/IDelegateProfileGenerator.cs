namespace Tapanga.Plugin;

/// <inheritdoc />
public interface IDelegateProfileGenerator : IProfileGenerator
{
    /// <summary>
    /// Whether the delegate is async.
    /// </summary>
    bool IsAsyncDelegate { get; }

    /// <summary>
    /// The type of the parameter passed to the delegate.
    /// </summary>
    Type ParameterType { get; }
}
