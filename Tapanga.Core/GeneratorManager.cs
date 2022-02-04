using Tapanga.Plugin;

namespace Tapanga.Core;

public class GeneratorManager
{
    private readonly IEnumerable<GeneratorFactoryAsync> _generatorFactories;
    private readonly ILogger _logger;
    private readonly bool _dryRun;

    public GeneratorManager(IEnumerable<GeneratorFactoryAsync> generatorFactories, ILogger logger, bool dryRun)
    {
        _generatorFactories = generatorFactories;
        _logger = logger;
        _dryRun = dryRun;
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
            result.Add(CreateGenerator(await factory(new GeneratorContext(_logger, ContextFileWriterImpl, ContextPathCombineImpl, _dryRun))));
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

    private static void ContextFileWriterImpl(IProfileGenerator generator, string relativePath, StringCollection content)
    {
        var readerWriter = GeneratorReaderWriter.Factory(generator.GetGeneratorId());
        readerWriter.WriteContent(relativePath, content);
    }

    private static string ContextPathCombineImpl(IProfileGenerator generator, string relativePath)
    {
        var rootPath = generator.GetGeneratorId().GetPluginSerializationDirectoryPath(Constants.FragmentsRootPath);
        return Path.Combine(rootPath, relativePath);
    }
}
