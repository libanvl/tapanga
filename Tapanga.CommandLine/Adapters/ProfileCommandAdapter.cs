﻿using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Rendering;
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

    public int ListHandler(SystemConsole console)
    {
        var terminal = console.GetTerminal();
        var renderer = new ConsoleRenderer(terminal);
        var view = new ProfileDataExItemsView(_profileManager.GetProfiles());
        terminal.Clear();
        view.Render(renderer, Region.Scrolling);
        console.Out.WriteLine();
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

    private int RemoveHandler(ColorConsole console, string id)
    {
        var result = _profileManager.RemoveProfile(id);
        var msg = result switch
        {
            ProfileManager.RemoveProfileResult.OK => $"OK: {id} removed",
            ProfileManager.RemoveProfileResult.NoMatchingProfile => $"No known profiles matched {id}",
            ProfileManager.RemoveProfileResult.MultipleProfiles => $"Multiple profiles matched {id}. Try using a longer prefix.",
            ProfileManager.RemoveProfileResult.DataLoadError => $"Tapanga failed to load profile data. Check that plugins are available at the expected path.",
            ProfileManager.RemoveProfileResult.Failed => $"Failed: {id} could not be removed.",
            _ => "Unknown error."
        };

        console.GreenLine(msg);
        return (int)result;
    }
}
