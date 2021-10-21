namespace Tapanga.Plugin;

public abstract class AsyncDelegateGenerator<T> : DelegateGeneratorBase<T, Task<int>> where T : IGeneratorArguments
{
    public override bool IsAsyncDelegate => true;
}
