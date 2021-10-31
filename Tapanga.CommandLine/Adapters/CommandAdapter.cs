using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Tapanga.Core;
using Tapanga.Plugin;

namespace Tapanga.CommandLine;

internal class CommandAdapter
{
    private readonly IProfileGeneratorAdapter _inner;
    private readonly ProfileDataCollection _profiles;

    public CommandAdapter(IProfileGeneratorAdapter profileGenerator, ProfileDataCollection profiles)
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

    private Command GetInfoCommand() => new Command("info")
    {
        Handler = CommandHandler.Create(InfoHandler),
        Description = $"Get extra information about the {_inner.GeneratorId.Key} generator",
    };

    private void InfoHandler(ColorConsole con)
    {
        var info = _inner.GeneratorInfo;
        con.MagentaLine(_inner.GeneratorId.Key);

        if (info.Any())
        {
            con.GreenLine(info);
        }
        else
        {
            con.YellowLine($"No info found for generator {_inner.GeneratorId.Key}");
        }
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
;
    }

    private int GoHandler(ColorConsole con)
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

        InfoHandler(con);
        con.WriteLine();

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
