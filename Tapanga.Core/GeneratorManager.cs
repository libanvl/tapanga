using Tapanga.Plugin;

namespace Tapanga.Core;

public delegate Task<IProfileGenerator> GeneratorFactoryAsync();

public class GeneratorManager
{
    private readonly IEnumerable<GeneratorFactoryAsync> _generatorFactories;

    public GeneratorManager(IEnumerable<GeneratorFactoryAsync> generatorFactories)
    {
        _generatorFactories = generatorFactories;
    }

    public async Task<IEnumerable<IProfileGeneratorAdapter>> GetProfileGeneratorsAsync()
    {
        if (!_generatorFactories.Any())
        {
            throw new InvalidOperationException("No generators are registered with the Generator Manager.");
        }

        var result = new List<IProfileGeneratorAdapter>();

        foreach (var factory in _generatorFactories)
        {
            result.Add(CreateGenerator(await factory()));
        }

        return result;
    }

    private static IProfileGeneratorAdapter CreateGenerator(IProfileGenerator profileGenerator)
    {
        return profileGenerator switch
        {
            IDelegateProfileGenerator dpg => new DelegateGeneratorAdapter(dpg),
            IProfileGenerator pg => new ProfileGeneratorAdapter(pg),
            _ => throw new TypeLoadException("Generator factory returned an unexpected type."),
        };
    }
}
