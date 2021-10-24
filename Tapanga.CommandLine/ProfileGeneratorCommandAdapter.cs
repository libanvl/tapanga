using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Tapanga.Plugin;

namespace Tapanga.CommandLine
{
    internal class ProfileGeneratorCommandAdapter
    {
        private readonly ProfileCollection _profiles;
        private readonly IProfileGenerator _inner;

        public ProfileGeneratorCommandAdapter(ProfileCollection profiles, IProfileGenerator profileGenerator)
        {
            _profiles = profiles;
            _inner = profileGenerator;
        }

        public Command GetCommand() => new(_inner.GeneratorInfo.ShortName, _inner.GeneratorInfo.LongName)
            {
                GetInfoCommand(),
                GetGoCommand(),
                GetRunCommand()
            };

        private Command GetInfoCommand() => new Command("info")
        {
            Handler = CommandHandler.Create(InfoHandler),
            Description = $"Get extra information about the {_inner.GeneratorInfo.LongName} generator",
        }.WithAlias("i");

        private void InfoHandler(ColorConsole con)
        {
            var info = _inner.GeneratorInfo;
            con.MagentaLine(info.LongName);

            if (info.Description is Opt<GeneratorDescription>.Some someDescription)
            {
                con.GreenLine(someDescription.Value);
            }
            else
            {
                con.YellowLine($"No description found for generator {_inner.GeneratorInfo.ShortName}");
            }
        }

        private Command GetRunCommand()
        {
            var runCommand = new Command("run")
            {
                Handler = CommandHandler.Create(this._inner.GetGeneratorDelegate(_profiles)),
                Description = $"Run the {this._inner.GeneratorInfo.LongName} generator using only command-line parameters",
            }
            .WithAlias("r");

            if (this._inner is IProvideUserArguments argProvider)
            {
                foreach (var userArg in argProvider.GetUserArguments())
                {
                    runCommand.AddOption(userArg.AsOption());
                }
            }

            return runCommand;
        }

        private Command GetGoCommand()
        {
            return new Command("go")
            {
                Handler = CommandHandler.Create(Handler),
                Description = $"Start the {_inner.GeneratorInfo.LongName} generator in interactive mode",
            };

            int Handler(ColorConsole con)
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
                    foreach (var arg in argProvider.GetUserArguments())
                    {
                        Option opt = arg.AsOption();

                        IValueDescriptor val = opt;

                        if (opt.ValueType.IsTypeOptionalArgument(out var boundType))
                        {
                            con.GreenLine($"{arg.Description} ({boundType.Name})");
                        }
                        else
                        {
                            con.GreenLine($"{arg.Description} ({opt.ValueType.Name})");
                        }

                        string response = string.Empty;
                        while (true)
                        {
                            con.Cyan(arg.LongName);

                            if (val.HasDefaultValue)
                            {
                                var defaultValue = val.GetDefaultValue();
                                if (defaultValue is OptionalArgument optional)
                                {
                                    defaultValue = optional.GetValue();
                                }

                                con.Blue($" [{defaultValue}]");
                            }

                            if (opt.IsRequired && !val.HasDefaultValue)
                            {
                                con.Yellow(" *REQUIRED*");
                            }

                            con.Cyan(": ");

                            response = Console.ReadLine() ?? string.Empty;
                            if (opt.IsRequired && !val.HasDefaultValue && string.IsNullOrEmpty(response))
                            {
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(response))
                        {
                            commandArgs.Add($"--{opt.Name}");
                            commandArgs.Add(response);
                        }

                        con.WriteLine();
                        command.Add(opt);
                    }
                }

                var argsArray = commandArgs.ToArray();
                var displayArgs = argsArray.Select(s => s.Any(char.IsWhiteSpace) ? $"\"{s}\"" : s);
                con.YellowLine($"Using: gen {_inner.GeneratorInfo.ShortName} run {string.Join(" ", displayArgs)}");

                return command.Invoke(argsArray);
            };
        }
    }
}
