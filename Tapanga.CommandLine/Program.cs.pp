using Tapanga.CommandLine;
using Tapanga.Core;

namespace $rootnamespace$;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Replace the values in the constructor below.
        return await new Runner("TOOL TITLE", Enumerable.Empty<GeneratorFactoryAsync>())
            .InvokeAsync(args);
    }
}