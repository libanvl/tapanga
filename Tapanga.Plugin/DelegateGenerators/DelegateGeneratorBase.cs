namespace Tapanga.Plugin;

public abstract class DelegateGeneratorBase<T, U> : IDelegateProfileGenerator where T : IGeneratorArguments
{
    public abstract GeneratorInfo GeneratorInfo { get; }

    public abstract bool IsAsyncDelegate { get; }

    protected abstract Func<T, U> GetGeneratorCore(IProfileCollection profiles);

    public virtual Type ParameterType => typeof(T);

    public virtual Delegate GetGeneratorDelegate(IProfileCollection profiles)
    {
        return this.GetGeneratorCore(profiles);
    }
}
