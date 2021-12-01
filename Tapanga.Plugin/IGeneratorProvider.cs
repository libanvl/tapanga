namespace Tapanga.Plugin;

/// <summary>
/// A factory method that creates an instance of <see cref="IProfileGenerator"/>.
/// </summary>
/// <param name="context">The generator creation context.</param>
public delegate Task<IProfileGenerator> GeneratorFactoryAsync(GeneratorContext context);

/// <summary>
/// Provides the generator factories.
/// </summary>
public interface IGeneratorProvider
{
    /// <summary>
    /// Gets the generator factories.
    /// </summary>
    IEnumerable<GeneratorFactoryAsync> GetGeneratorFactories();
}
