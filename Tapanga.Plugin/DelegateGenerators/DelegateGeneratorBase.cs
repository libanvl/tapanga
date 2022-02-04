namespace Tapanga.Plugin;

/// <summary>
/// The base default delegate generator.
/// </summary>
/// <typeparam name="T">The type of the Parameter passed to the delegate.</typeparam>
/// <typeparam name="U">The return type of the delegate.</typeparam>
public abstract class DelegateGeneratorBase<T, U> : IDelegateProfileGenerator
{
    /// <inheritdoc />
    public abstract StringCollection GeneratorInfo { get; }

    /// <inheritdoc />
    public abstract bool IsAsyncDelegate { get; }

    /// <inheritdoc />
    public virtual Type ParameterType => typeof(T);

    /// <inheritdoc />
    public virtual Delegate GetGeneratorDelegate(IProfileDataCollection profiles)
    {
        return (T arguments) => this.GeneratorCore(profiles, arguments);
    }

    /// <summary>
    /// Override to provide an implementation for the generator delegate.
    /// </summary>
    /// <param name="profiles">Add new profiles to this collection</param>
    /// <param name="args">The generator parameters</param>
    /// <returns>A value indicating the result of the generation.</returns>
    protected abstract U GeneratorCore(IProfileDataCollection profiles, T args);
}
