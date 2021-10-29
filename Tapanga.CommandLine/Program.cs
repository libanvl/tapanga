using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Reflection;
using Tapanga.Core;

namespace Tapanga.CommandLine;

internal class Program
{
    internal const string RootDescription = "TaPaN-Ga: Terminal Profile N-Generator";

    private static readonly Option<IEnumerable<DirectoryInfo>> PluginPathOption =
        new Option<IEnumerable<DirectoryInfo>>(
            "--plugin-path",
            description: "Directory to search for plugin assemblies. Accepts mutiple options.",
            getDefaultValue: () => new[] { new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly()?.Location)!) })
        {
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.ExactlyOne,
        }
        .ExistingOnly()
        .WithAlias("-pp");

    public static async Task<int> Main(string[] args)
    {
        try
        {
            var console = new ColorConsole();
            

            ParseResult parseResult = new Parser(PluginPathOption).Parse(args);
            IEnumerable<DirectoryInfo>? pluginPaths = parseResult.ValueForOption(PluginPathOption)?
                .Where(di => di.Exists);

            var pm = new ProfileManager();
            var gm = new GeneratorManager(s => console.YellowLine(s), pluginPaths.WrapOpt(emptyIsNone: true));

            var profiles = pm.LoadProfiles();

            int ret = await BuildCommandLine(gm, profiles)
                .UseDefaults()
                .BuildServiceMiddleware(console)
                .Build()
                .InvokeAsync(args);

            if (ret != 0)
            {
                return ret;
            }

            foreach (var collection in profiles.Values)
            {
                foreach (var p in collection)
                {
                    Console.WriteLine($"PROFILE: {p}");
                }
            }

            new ProfileManager().WriteProfiles(profiles);

            return 0;
        }
        finally
        {
            Console.ResetColor();
        }
    }

    private static CommandLineBuilder BuildCommandLine(GeneratorManager gm, IDictionary<GeneratorId, ProfileDataCollection> profilesMap)
    {
        var rootCommand = new RootCommand(RootDescription)
        {
            PluginPathOption,
        };

        var generatorCommand = new Command("generator")
        {
            Description = "Interact with the available generators",
        };

        foreach (var pg in gm.GetProfileGenerators())
        {
            if (!profilesMap.TryGetValue(pg.GeneratorId, out var profiles))
            {
                profiles = new ProfileDataCollection();
                profilesMap[pg.GeneratorId] = profiles;
            }

            var innerCommands = new ProfileGeneratorCommandAdapter(profiles, pg);
            generatorCommand.Add(innerCommands.GetCommand());
        }

        rootCommand.Add(generatorCommand.WithAlias("gen", "g"));

        return new CommandLineBuilder(rootCommand);
    }
}