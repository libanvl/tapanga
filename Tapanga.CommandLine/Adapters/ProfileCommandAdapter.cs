using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using Tapanga.Core;

namespace Tapanga.CommandLine;

internal class ProfileCommandAdapter
{
    private readonly ProfileManager _profileManager;

    public ProfileCommandAdapter(ProfileManager profileManager)
    {
        _profileManager = profileManager;
    }

    public Command GetCommand() => new("profile", "Manage the generated profiles")
    {
        GetListCommand().WithAlias("ls"),
        GetRemoveCommand().WithAlias("rm")
    };

    private Command GetListCommand() => new("list")
    {
        Handler = CommandHandler.Create(ListHandler),
        Description = "List all profiles created from the loaded plugins"
    };

    private int ListHandler(SystemConsole console)
    {
        var terminal = console.GetTerminal();
        var renderer = new ConsoleRenderer(terminal);
        var view = new ProfileDataExItemsView(_profileManager.GetProfiles());
        terminal.Clear();
        view.Render(renderer, Region.Scrolling);
        return 0;
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

    private int RemoveHandler(string id)
    {
        return (int)_profileManager.RemoveProfile(id);
    }
}
