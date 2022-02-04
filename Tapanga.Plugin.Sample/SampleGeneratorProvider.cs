namespace Tapanga.Plugin.Sample;

public class SampleGeneratorProvider : IGeneratorProvider
{
    public IEnumerable<GeneratorFactoryAsync> GetGeneratorFactories()
    {
        yield return context => Task.FromResult<IProfileGenerator>(new SampleDelegateGenerator(context));
    }
}
