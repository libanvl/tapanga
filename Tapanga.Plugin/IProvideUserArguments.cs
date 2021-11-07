using libanvl;

namespace Tapanga.Plugin;

/// <summary>
/// Provides <see cref="UserArgument"/>s that are bound to the parameters of the
/// <see cref="IProfileGenerator.GetGeneratorDelegate(IProfileDataCollection)"/> delegate.
/// </summary>
public interface IProvideUserArguments
{
    /// <summary>
    /// Gets the user arguments for the generator.
    /// </summary>
    public IReadOnlyList<UserArgument> GetUserArguments();
}

/// <summary>
/// An abstract UserArgument.
/// </summary>
/// <param name="ValueType">The desired type of the input</param>
/// <param name="LongName">The long name</param>
/// <param name="ShortName">An optional short name</param>
/// <param name="Description">A description</param>
/// <param name="DefaultObject">The default value,or <c>null</c> if no default value</param>
/// <param name="Required">Whether the argument is required.</param>
/// <param name="Sort">The sort order of argument.</param>
public abstract record UserArgument(
    Type ValueType,
    string LongName,
    Opt<string> ShortName,
    string Description,
    object? DefaultObject,
    bool Required,
    int Sort);

/// <inheritdoc/>
public record UserArgument<T>(
    string LongName,
    Opt<string> ShortName,
    string Description,
    Opt<T> Default,
    bool Required,
    int Sort = 0)
    : UserArgument(typeof(T), LongName, ShortName, Description, Default.SomeOrDefault(default!), Required, Sort);
