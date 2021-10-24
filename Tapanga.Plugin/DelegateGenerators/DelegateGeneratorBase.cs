namespace Tapanga.Plugin;

public abstract class DelegateGeneratorBase<T, U> : IDelegateProfileGenerator where T : IGeneratorArguments
{
    public abstract GeneratorInfo GeneratorInfo { get; }

    public abstract bool IsAsyncDelegate { get; }

    protected abstract U GeneratorCore(IProfileCollection profiles, T args);

    public virtual Type ParameterType => typeof(T);

    public virtual Delegate GetGeneratorDelegate(IProfileCollection profiles)
    {
        return (T arguments) => this.GeneratorCore(profiles, arguments);
    }
}
