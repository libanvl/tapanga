using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Reflection;
using Tapanga.Core;

namespace Tapanga.CommandLine;

public class Program
{
    internal const string RootDescription = "TaPaN-Ga: Terminal Profile N-Generator";

    private static readonly Option<IEnumerable<DirectoryInfo>> PluginPathOption =
        new Option<IEnumerable<DirectoryInfo>>(
            "--plugin-path",
            description: "Directory to search for plugin assemblies. Multiple options are allowed.",
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
            var profiles = new ProfileCollection();

            ParseResult parseResult = new Parser(PluginPathOption).Parse(args);
            IEnumerable<DirectoryInfo>? pluginPaths = parseResult.ValueForOption(PluginPathOption)?
                .Where(di => di.Exists);

            var gm = new GeneratorManager(s => console.YellowLine(s), pluginPaths.WrapOpt(emptyIsNone: true));

            int ret = await BuildCommandLine(gm, profiles)
                .UseDefaults()
                .BuildServiceMiddleware(console)
                .Build()
                .InvokeAsync(args);

            if (ret != 0)
            {
                return ret;
            }

            foreach (var p in profiles)
            {
                Console.WriteLine($"PROFILE: {p}");
            }

            return 0;
        }
        finally
        {
            Console.ResetColor();
        }
    }

    private static CommandLineBuilder BuildCommandLine(GeneratorManager gm, ProfileCollection profiles)
    {
        var rootCommand = new RootCommand(RootDescription)
        {
            PluginPathOption,
        };

        IEnumerable<Plugin.IProfileGenerator> profileGenerators = gm.GetProfileGenerators();
        if (profileGenerators.Any())
        {
            var generatorCommand = new Command("generator")
            {
                Description = "Interact with the available generators",
            };
            foreach (var pg in profileGenerators)
            {
                var innerCommands = new ProfileGeneratorCommandAdapter(profiles, pg);
                generatorCommand.Add(innerCommands.GetCommand());
            }

            rootCommand.Add(generatorCommand.WithAlias("gen", "g"));
        }
        else
        {
            rootCommand.Handler = CommandHandler.Create(() => Console.WriteLine("No profile generators found!"));
        }

        return new CommandLineBuilder(rootCommand);
    }
}