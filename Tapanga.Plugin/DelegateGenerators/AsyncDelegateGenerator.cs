namespace Tapanga.Plugin;

public abstract class AsyncDelegateGenerator<T> : DelegateGeneratorBase<T, Task<int>>
{
    public override bool IsAsyncDelegate => true;
}
