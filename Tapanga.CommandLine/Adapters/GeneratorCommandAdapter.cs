﻿using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.CommandLine.Rendering;
using Tapanga.Core;
using Tapanga.Plugin;

namespace Tapanga.CommandLine;

internal class GeneratorCommandAdapter
{
    private readonly IProfileGeneratorAdapter _inner;
    private readonly ProfileDataCollection _profiles;

    public GeneratorCommandAdapter(IProfileGeneratorAdapter profileGenerator, ProfileDataCollection profiles)
    {
        _inner = profileGenerator;
        _profiles = profiles;
    }

    public Command GetCommand() => new(_inner.GeneratorId.Key, _inner.Description)
    {
        GetInfoCommand(),
        GetGoCommand(),
        GetRunCommand()
    };

    private Command GetInfoCommand() => new("info")
    {
        Handler = CommandHandler.Create(InfoHandler),
        Description = $"Get extra information about the {_inner.GeneratorId.Key} generator",
    };

    private void InfoHandler(SystemConsole console)
    {
        var terminal = console.GetTerminal();
        terminal.Clear();
        var view = new ProfileGeneratorView(_inner);
        view.Render(new ConsoleRenderer(terminal, resetAfterRender: true), Region.Scrolling);
    }

    private Command GetRunCommand()
    {
        var runCommand = new Command("run")
        {
            Handler = CommandHandler.Create(_inner.GetGeneratorDelegate(_profiles)),
            Description = $"Run the {_inner.GeneratorId.Key} generator using only command-line parameters",
        };

        if (_inner is IProvideUserArguments argProvider)
        {
            foreach (var opt in argProvider.GetUserArguments().Select(ua => new OptionAdapter(ua)))
            {
                runCommand.AddOption(opt);
            }
        }

        return runCommand;
    }

    private Command GetGoCommand()
    {
        return new Command("go")
        {
            Handler = CommandHandler.Create(GoHandler),
            Description = $"Start the {_inner.GeneratorId.Key} generator in interactive mode",
        };
    }

    private int GoHandler(SystemConsole systemConsole, ColorConsole con)
    {
        var command = new RootCommand("__internal_go_handler__")
        {
            Handler = CommandHandler.Create(_inner.GetGeneratorDelegate(_profiles)),
            IsHidden = true,
        };

        var commandArgs = new List<string>();

        con.DarkBlue(Program.RootDescription, ConsoleColor.Gray);
        con.WriteLine();
        con.RedLine("Ctrl-C to cancel");

        InfoHandler(systemConsole);
        systemConsole.Out.WriteLine();
        systemConsole.Out.WriteLine();

        if (_inner is IProvideUserArguments argProvider)
        {
            foreach (var opt in argProvider.GetUserArguments().Select(arg => new OptionAdapter(arg)))
            {
                con.GreenLine($"{opt.Description} ({opt.BoundType.Name})");

                string response = string.Empty;
                while (true)
                {
                    WritePrompt(con, opt);

                    response = Console.ReadLine() ?? string.Empty;

                    if (opt.IsRequired
                        && !opt.HasDefaultValue
                        && string.IsNullOrEmpty(response))
                    {
                        continue;
                    }

                    break;
                }

                if (!string.IsNullOrEmpty(response))
                {
                    commandArgs.Add($"--{opt.Name}");

                    if (opt.Arity.MaximumNumberOfValues > 1 && !Utilities.IsQuoted(response))
                    {
                            commandArgs.AddRange(response.Split(' '));
                    }
                    else
                    {
                        commandArgs.Add(response);
                    }
                }

                con.WriteLine();
                command.Add(opt);
            }
        }

        return command.Invoke(commandArgs.ToArray());

        static void WritePrompt(ColorConsole con, OptionAdapter opt)
        {
            con.Cyan(opt.LongName);

            if (opt.HasDefaultValue)
            {
                con.Blue($" [{opt.GetDefaultValuesString()}]");
            }

            if (opt.IsRequired && !opt.HasDefaultValue)
            {
                con.Yellow(" *REQUIRED*");
            }

            con.Cyan(": ");
        }
    }
}
