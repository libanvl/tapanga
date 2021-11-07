namespace Tapanga.Plugin;

/// <summary>
/// Base for an asynchronous delegate generator.
/// </summary>
/// <typeparam name="T">The type of the Parameter passed to the delegate.</typeparam>
public abstract class AsyncDelegateGenerator<T> : DelegateGeneratorBase<T, Task<int>>
{
    /// <inheritdoc />
    public override bool IsAsyncDelegate => true;
}
