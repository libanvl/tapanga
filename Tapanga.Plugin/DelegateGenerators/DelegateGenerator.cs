namespace Tapanga.Plugin;

public abstract class DelegateGenerator<T> : DelegateGeneratorBase<T, int>
{
    public override bool IsAsyncDelegate => false;

    protected static U NotNullOrThrow<U>(U? value)
    {
        if (value is null)
        {
            throw new ArgumentException();
        }

        return value;
    }
}
