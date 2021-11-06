using libanvl;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using Tapanga.Core;

namespace Tapanga.CommandLine;

internal class Program
{
    internal const string RootDescription = "TaPaN-Ga: Terminal Profile N-Generator";

    private static readonly Option<IEnumerable<DirectoryInfo>> PluginPathOption =
        new Option<IEnumerable<DirectoryInfo>>("--plugin-path", getDefaultValue: () => new[] { new DirectoryInfo(AppContext.BaseDirectory) })
        {
            Description = "Directory to search for plugin assemblies. Accepts mutiple options.",
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
            IEnumerable<DirectoryInfo>? pluginPaths = parseResult
                .ValueForOption(PluginPathOption)?
                .Where(di => di.Exists);

            var gm = new GeneratorManager(pluginPaths.WrapOpt(emptyIsNone: true));
            var pm = new ProfileManager(gm);

            var newProfilesMap = new ProfileDataCollectionMap();

            int ret = await BuildCommandLine(gm, pm, newProfilesMap)
                .UseDefaults()
                .BuildServiceMiddleware(console)
                .Build()
                .InvokeAsync(args);

            if (ret != 0)
            {
                return ret;
            }

            pm.AddProfileData(newProfilesMap);

            if (parseResult.Directives.Contains("dry-run"))
            {
                new ProfileCommandAdapter(pm).ListHandler(new SystemConsole());
                console.MagentaLine("Running in Dry Run mode");
            }
            else
            {
                return pm.Write() ? 0 : -1;
            }

            return 0;
        }
        catch (Exception ex)
        {
            console.RedLine(ex.Message);
            return -2;
        }
        finally
        {
            console.Reset();
        }
    }

    private static CommandLineBuilder BuildCommandLine(GeneratorManager gm, ProfileManager pm, ProfileDataCollectionMap newProfilesMap)
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
            var innerCommands = new GeneratorCommandAdapter(pg, newProfilesMap[pg.GeneratorId]);
            generatorCommand.Add(innerCommands.GetCommand());
        }

        rootCommand.Add(generatorCommand.WithAlias("gen"));

        var profileCommandAdapter = new ProfileCommandAdapter(pm);
        rootCommand.Add(profileCommandAdapter.GetCommand().WithAlias("pro"));

        return new CommandLineBuilder(rootCommand);
    }
}