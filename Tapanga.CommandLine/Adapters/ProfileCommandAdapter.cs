using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using Tapanga.Core;

namespace Tapanga.CommandLine;

internal class ProfileCommandAdapter
{
    private readonly SerializationManager _serializationManager;
    private readonly GeneratorId _generatorId;

    public ProfileCommandAdapter(SerializationManager serializationManager, GeneratorId generatorId)
    {
        _serializationManager = serializationManager;
        _generatorId = generatorId;
    }

    public Command GetCommand() => new("profile", "Manage the generated profiles")
    {
        GetListCommand().WithAlias("ls"),
        GetRemoveCommand().WithAlias("rm"),
        GetClearGeneratorCommand()
    };

    private Command GetListCommand() => new("list")
    {
        Handler = CommandHandler.Create(ListHandler),
        Description = "List all profiles created from the loaded plugins"
    };

    public int ListHandler(ColorConsole console)
    {
        if (_serializationManager.TryLoad(out var profiles))
        {
            var filtered = profiles.Where(pde => pde.GeneratorId == _generatorId);
            var view = new ProfileDataExItemsView(filtered);
            var terminal = console.GetTerminal();

            terminal.Clear();
            view.Render(new ConsoleRenderer(terminal), Region.Scrolling);
            console.WriteLine();
            return 0;
        }

        return -1;
    }

    private Command GetRemoveCommand()
    {
        Command command = new("remove")
        {
            new Argument<string>("id", "the id of the profile to remove")
        };

        command.Handler = CommandHandler.Create(RemoveHandler);

        return command;
    }

    private int RemoveHandler(ColorConsole console, string id)
    {
        var result = _serializationManager.RemoveProfile(_generatorId, id);
        var msg = result switch
        {
            RemoveProfileResult.OK => $"OK: {id} removed",
            RemoveProfileResult.NoMatchingProfile => $"No known profiles matched {id}",
            RemoveProfileResult.MultipleProfiles => $"Multiple profiles matched {id}. Try using a longer prefix.",
            RemoveProfileResult.DataLoadError => $"Tapanga failed to load profile data. Check that plugins are available at the expected path.",
            RemoveProfileResult.Failed => $"Failed: {id} could not be removed.",
            _ => "Unknown error."
        };

        console.GreenLine(msg);
        return (int)result;
    }

    private Command GetClearGeneratorCommand()
    {
        return new("clear-generator-profiles")
        {
            Handler = CommandHandler.Create(ClearGeneratorHandler),
        };
    }

    private int ClearGeneratorHandler(ColorConsole console)
    {
        var results = _serializationManager.RemoveGeneratorProfiles(_generatorId.Key);
        if (results.All(r => r == RemoveProfileResult.OK))
        {
            console.GreenLine($"OK: {results.Count()} profiles removed");
            return 0;
        }

        console.MagentaLine("Errors encountered.");
        return -1;
    }
}
