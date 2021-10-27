namespace Tapanga.Plugin;

/// <summary>
/// A Terminal profile generator.
/// </summary>
public interface IProfileGenerator
{
    /// <summary>
    /// Additional information about the profile generator.
    /// </summary>
    public GeneratorInfo GeneratorInfo { get; }

    /// <summary>
    /// Get the <see cref="Delegate"/> that does the work of the generator.
    /// </summary>
    /// <param name="profiles">The generator adds <see cref="ProfileData"/> to this collection</param>
    /// <returns>The generator delegate</returns>
    public Delegate GetGeneratorDelegate(IProfileCollection profiles);
}

/// <summary>
/// Provides <see cref="UserArgument"/>s that are bound to the parameters of the
/// <see cref="IProfileGenerator.GetGeneratorDelegate(IProfileCollection)"/> delegate.
/// </summary>
public interface IProvideUserArguments
{
    public IReadOnlyList<UserArgument> GetUserArguments();
}

/// <summary>
/// An abstract UserArgument.
/// </summary>
/// <param name="LongName">The long name</param>
/// <param name="ShortName">The short name</param>
/// <param name="Description">A description</param>
/// <param name="Required">Whether the argument is required.</param>
/// <param name="Sort">The sort order of argument.</param>
public abstract record UserArgument(string LongName, Opt<string> ShortName, string Description, bool Required, int Sort);

/// <inheritdoc/>
/// <param name="Default">An optional default value.</param>
public record UserArgument<T>(string LongName, Opt<string> ShortName, string Description, Opt<T> Default, bool Required, int Sort = 0)
    : UserArgument(LongName, ShortName, Description, Required, Sort);

public record ProfileData(string Name, string CommandLine, Opt<DirectoryInfo> StartingDirectory, Opt<string> TabTitle, Opt<Icon> Icon);

public abstract record Icon(string Name);

public record StreamIcon(string Name, Stream Stream) : Icon(Name);

public record PathIcon(string Name, string Path) : Icon(Name);
