namespace Tapanga.Plugin;

public abstract class DelegateGeneratorBase<T, U> : IDelegateProfileGenerator
{
    public abstract GeneratorInfo GeneratorInfo { get; }

    public abstract bool IsAsyncDelegate { get; }

    protected abstract U GeneratorCore(IProfileDataCollection profiles, T args);

    public virtual Type ParameterType => typeof(T);

    public virtual Delegate GetGeneratorDelegate(IProfileDataCollection profiles)
    {
        return (T arguments) => this.GeneratorCore(profiles, arguments);
    }
}
