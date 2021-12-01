using Tapanga.Plugin;

namespace Tapanga.Core;

public class GeneratorManager
{
    private readonly IEnumerable<GeneratorFactoryAsync> _generatorFactories;
    private readonly GeneratorContext _context;

    public GeneratorManager(IEnumerable<GeneratorFactoryAsync> generatorFactories, GeneratorContext context)
    {
        _generatorFactories = generatorFactories;
        _context = context;
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
            result.Add(CreateGenerator(await factory(_context)));
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
