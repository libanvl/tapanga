using Tapanga.CommandLine;
using Tapanga.Plugin.Sample;

await new Runner("Test platform for tapanga features", new SampleGeneratorProvider())
    .InvokeAsync(args);
