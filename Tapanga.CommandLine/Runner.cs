using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using Tapanga.Core;

namespace Tapanga.CommandLine;

/// <summary>
/// The Tapanga CommandLine Runner.
/// </summary>
public class Runner
{
    private readonly string _rootDescription;
    private readonly GeneratorManager _generatorManager;

    /// <summary>
    /// Initializes a new instance of <see cref="Runner"/>.
    /// </summary>
    /// <param name="rootDescription">A name for the tool. Displayed in menus and outputs.</param>
    /// <param name="generatorFactories">Factories for the tool generators.</param>
    public Runner(string rootDescription, IEnumerable<GeneratorFactoryAsync> generatorFactories)
    {
        _rootDescription = rootDescription;
        _generatorManager = new GeneratorManager(generatorFactories);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Runner"/>.
    /// </summary>
    /// <param name="rootDescription">A name for the tool. Displayed in menus and outputs.</param>
    /// <param name="generatorFactories">Factories for the tool generators.</param>
    public Runner(string rootDescription, params GeneratorFactoryAsync[] generatorFactories)
        : this(rootDescription, generatorFactories.AsEnumerable())
    {
    }

    /// <summary>
    /// Invoke the command-line tool.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>An exit code.</returns>
    public async Task<int> InvokeAsync(string[] args)
    {
        var console = new ColorConsole();

        try
        {
            ParseResult parseResult = new Parser().Parse(args);

            var generators = await _generatorManager.GetProfileGeneratorsAsync();

            var serializationManager = new SerializationManager(generators);

            var newProfilesMap = new ProfileDataCollectionMap();

            int ret = await BuildCommandLine(generators, serializationManager, newProfilesMap)
                .UseDefaults()
                .BuildServiceMiddleware(console)
                .Build()
                .InvokeAsync(args);

            if (ret != 0)
            {
                return ret;
            }

            serializationManager.AddProfileData(newProfilesMap);

            if (parseResult.Directives.Contains("dry-run"))
            {
                new ProfileCommandAdapter(serializationManager).ListHandler(new SystemConsole());
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
            console.Reset();
        }
    }

    private CommandLineBuilder BuildCommandLine(IEnumerable<IProfileGeneratorAdapter> generators, SerializationManager pm, ProfileDataCollectionMap newProfilesMap)
    {
        var rootCommand = new RootCommand(_rootDescription);

        foreach (var pg in generators)
        {
            var innerCommands = new GeneratorCommandAdapter(_rootDescription, pg, newProfilesMap[pg.GeneratorId]);
            rootCommand.Add(innerCommands.GetCommand());
        }

        var profileCommandAdapter = new ProfileCommandAdapter(pm);
        rootCommand.Add(profileCommandAdapter.GetCommand().WithAlias("pro"));

        return new CommandLineBuilder(rootCommand);
    }
}
