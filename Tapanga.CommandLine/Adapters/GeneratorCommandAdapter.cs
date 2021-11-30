using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.CommandLine.Rendering;
using Tapanga.Core;
using Tapanga.Plugin;

namespace Tapanga.CommandLine;

internal class GeneratorCommandAdapter
{
    private readonly string _rootDescription;
    private readonly IProfileGeneratorAdapter _inner;
    private readonly ProfileDataCollection _profiles;
    private readonly ProfileCommandAdapter _pga;

    public GeneratorCommandAdapter(
        string rootDescription,
        IProfileGeneratorAdapter profileGenerator,
        ProfileDataCollection profiles,
        ProfileCommandAdapter pga)
    {
        _rootDescription = rootDescription;
        _inner = profileGenerator;
        _profiles = profiles;
        _pga = pga;
    }

    public RootCommand GetRootCommand() => new(_rootDescription)
    {
        GetInfoCommand(),
        GetGoCommand(),
        GetRunCommand(),
        _pga.GetCommand().WithAlias("pro"),
    };

    public Command GetCommands() => new(_inner.GeneratorId.Key, _inner.Description)
    {
        GetInfoCommand(),
        GetGoCommand(),
        GetRunCommand(),
        _pga.GetCommand().WithAlias("pro"),
    };

    private Command GetInfoCommand() => new("info")
    {
        Handler = CommandHandler.Create(InfoHandler),
        Description = $"Get more information about the {_inner.GeneratorId.Key} generator",
    };

    private void InfoHandler(ColorConsole console)
    {
        var terminal = console.GetTerminal(preferVirtualTerminal: true);
        terminal.Clear();
        var view = new GeneratorView(_inner);
        view.Render(new ConsoleRenderer(terminal, resetAfterRender: true), Region.EntireTerminal);
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

    private int GoHandler(ColorConsole console)
    {
        var command = new RootCommand("__internal_go_handler__")
        {
            Handler = CommandHandler.Create(_inner.GetGeneratorDelegate(_profiles)),
            IsHidden = true,
        };

        var commandArgs = new List<string>();
        console.DarkBlue(_rootDescription, ConsoleColor.Gray);
        console.WriteLine();
        console.RedLine("Ctrl-C to cancel");

        InfoHandler(console);
        console.WriteLine();
        console.WriteLine();

        if (_inner is IProvideUserArguments argProvider)
        {
            foreach (var opt in argProvider.GetUserArguments().Select(arg => new OptionAdapter(arg)))
            {
                console.GrayLine($"{opt.Description} ({opt.BoundType.Name})");

                if (opt.Arity.MaximumNumberOfValues > 1)
                {
                    console.MagentaLine($">> Seperate multiple inputs with space <<");
                }

                string response = string.Empty;
                while (true)
                {
                    WriteOptPrompt(console, opt);

                    response = ColorConsole.ReadLine() ?? string.Empty;

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

                console.WriteLine();
                command.Add(opt);
            }
        }

        var result = command.Invoke(commandArgs.ToArray());

        if (result == 0)
        {
            console.CyanLine(">> Success!");
            console.CyanLine(">> Restart Windows Terminal for changes to take effect.");
        }

        return result;

        static void WriteOptPrompt(ColorConsole console, OptionAdapter opt)
        {
            console.Cyan(opt.LongName);

            if (opt.HasDefaultValue)
            {
                console.Blue($" [{opt.GetDefaultValuesString()}]");
            }

            if (opt.IsRequired && !opt.HasDefaultValue)
            {
                console.Yellow(" *REQUIRED*");
            }

            console.Cyan(": ");
        }
    }
}
