using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.CommandLine.Rendering;
using Tapanga.Core;
using Tapanga.Plugin;

namespace Tapanga.CommandLine;

/// <summary>
/// The Tapanga CommandLine Runner.
/// </summary>
public class Runner
{
    private readonly string _rootDescription;
    private readonly IEnumerable<GeneratorFactoryAsync> _generatorFactories;

    /// <summary>
    /// Initializes a new instance of <see cref="Runner"/>.
    /// </summary>
    /// <param name="rootDescription">A name for the tool. Displayed in menus and outputs.</param>
    /// <param name="generatorProvider">A generator provider.</param>
    public Runner(string rootDescription, IGeneratorProvider generatorProvider)
    {
        _rootDescription = rootDescription;
        _generatorFactories = generatorProvider.GetGeneratorFactories();
    }

    /// <summary>
    /// Invoke the command-line tool.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>An exit code.</returns>
    public async Task<int> InvokeAsync(string[] args)
    {
        var console = new ColorConsole(new SystemConsole());

        try
        {
            ParseResult parseResult = new Parser().Parse(args);
            bool dryrun = parseResult.Directives.Contains("dryrun");

            var logger = new ColorConsoleLogger(console, GetLogLevel(parseResult.Directives));
            var generatorContext = new GeneratorContext(logger, dryrun);
            var generatorManager = new GeneratorManager(_generatorFactories, generatorContext);

            var generators = await generatorManager.GetProfileGeneratorsAsync();
            var serializationManager = new SerializationManager(generators);

            var newProfilesMap = new ProfileDataCollectionMap();

            int ret = await BuildCommandLine(generators, serializationManager, newProfilesMap)
                .UseDefaults()
                .BuildServiceMiddleware()
                .Build()
                .InvokeAsync(args);

            if (ret != 0)
            {
                return ret;
            }

            serializationManager.AddProfileData(newProfilesMap);

            if (dryrun)
            {
                if (serializationManager.TryLoad(out var profiles))
                {
                    var view = new ProfileDataExItemsView(profiles);
                    console.Append(view);
                }

                console.MagentaLine("Running in Dry Run mode");
            }
            else
            {
                return serializationManager.Write() ? 0 : Errors.SerializationError;
            }

            return 0;
        }
        catch (Exception ex)
        {
            console.RedLine(ex.Message);
            return Errors.UnknownError;
        }
        finally
        {
            console.ResetColor();
        }
    }

    private CommandLineBuilder BuildCommandLine(
        IEnumerable<IProfileGeneratorAdapter> generators,
        SerializationManager sm,
        ProfileDataCollectionMap newProfilesMap)
    {
        var rootCommand = new RootCommand(_rootDescription);

        if (generators.Count() == 1 && generators.FirstOrDefault() is IProfileGeneratorAdapter pga)
        {
            var profileDataCollection = newProfilesMap[pga.GeneratorId];
            var gca = new GeneratorCommandAdapter(
                _rootDescription,
                pga,
                profileDataCollection,
                new ProfileCommandAdapter(sm, pga.GeneratorId));

            rootCommand = gca.GetRootCommand();
        }
        else
        {
            foreach (var pg in generators)
            {
                var innerCommands = new GeneratorCommandAdapter(
                    _rootDescription,
                    pg,
                    newProfilesMap[pg.GeneratorId],
                    new ProfileCommandAdapter(sm, pg.GeneratorId));

                rootCommand.Add(innerCommands.GetCommands());
            }
        }

        return new CommandLineBuilder(rootCommand);
    }

    private static LogLevel GetLogLevel(IDirectiveCollection directives)
    {
        if (directives.Contains("verbose"))
        {
            return LogLevel.Verbose;
        }

        if (directives.Contains("info"))
        {
            return LogLevel.Information;
        }

        if (directives.Contains("warning"))
        {
            return LogLevel.Warning;
        }

        return LogLevel.Error;
    }
}
