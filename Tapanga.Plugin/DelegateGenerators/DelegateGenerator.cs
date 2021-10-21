namespace Tapanga.Plugin;

public abstract class DelegateGenerator<T> : DelegateGeneratorBase<T, int> where T : IGeneratorArguments
{
    public override bool IsAsyncDelegate => false;
}
