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
            getDefaultValue: () => new[]
            { 
                new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly()?.Location)!)
            }
        )
        {
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.ExactlyOne,
        }
        .ExistingOnly()
        .WithAlias("-pp");

    public static async Task<int> Main(string[] args)
    {
        var console = new ColorConsole();

        try
        {
            ParseResult parseResult = new Parser(PluginPathOption).Parse(args);
            IEnumerable<DirectoryInfo>? pluginPaths = parseResult.ValueForOption(PluginPathOption)?
                .Where(di => di.Exists);

            var gm = new GeneratorManager(s => console.YellowLine(s), pluginPaths.WrapOpt(emptyIsNone: true));
            var pm = new ProfileManager(gm);

            var profilesMap = pm.LoadProfiles();

            int ret = await BuildCommandLine(gm, profilesMap)
                .UseDefaults()
                .BuildServiceMiddleware(console)
                .Build()
                .InvokeAsync(args);

            if (ret != 0)
            {
                return ret;
            }

            if (parseResult.Directives.Contains("dry-run"))
            {
                console.MagentaLine("Running in Dry Run mode");
                foreach (var (generatorId, profiles) in profilesMap)
                {
                    console.CyanLine(generatorId, ConsoleColor.DarkBlue);

                    foreach (var p in profiles)
                    {
                        console.GrayLine(p, ConsoleColor.DarkBlue);
                    }
                }
            }
            else
            {
                pm.WriteProfiles(profilesMap);
            }

            return 0;
        }
        catch (Exception ex)
        {
            console.RedLine(ex.Message);
            return -1;
        }
        finally
        {
            console.Reset();
        }
    }

    private static CommandLineBuilder BuildCommandLine(GeneratorManager gm, ProfileCollectionMap profilesMap)
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
            var innerCommands = new CommandAdapter(pg, profilesMap[pg.GeneratorId]);
            generatorCommand.Add(innerCommands.GetCommand());
        }

        rootCommand.Add(generatorCommand.WithAlias("gen"));

        return new CommandLineBuilder(rootCommand);
    }
}