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
    public Delegate GetGeneratorDelegate(IProfileDataCollection profiles);
}
