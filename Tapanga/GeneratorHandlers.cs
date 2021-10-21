using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Tapanga.Plugin;

namespace Tapanga.CommandLine
{
    internal static class GeneratorHandlers
    {
        public static Action<ColorConsole> GetInfoHandler(this IProfileGenerator gen)
        {
            return (ColorConsole con) =>
            {
                var info = gen.GeneratorInfo;
                con.MagentaLine(info.LongName);

                if (info.Description is Opt<GeneratorDescription>.Some someDescription)
                {
                    con.GreenLine(someDescription.Value);
                }
                else
                {
                    con.YellowLine($"No description found for generator {gen.GeneratorInfo.ShortName}");
                }
            };
        }

        public static Func<ColorConsole, int> GetGoHandler(this IProfileGenerator gen, ProfileCollection profiles)
        {
            return (ColorConsole con) =>
            {
                var command = new Command("__internal_go_handler__")
                {
                    Handler = CommandHandler.Create(gen.GetGeneratorDelegate(profiles)),
                    IsHidden = true
                };

                var commandArgs = new List<string>();

                con.DarkBlue(Program.RootDescription, ConsoleColor.Gray);
                con.WriteLine();
                con.RedLine("Ctrl-C to cancel");

                gen.GetInfoHandler()(con);
                con.WriteLine();

                if (gen is IProvideUserArguments argProvider)
                {
                    foreach (var arg in argProvider.GetUserArguments().OrderBy(ua => ua.Sort))
                    {
                        Option opt = arg.AsOption();
                        IValueDescriptor val = opt;

                        if (opt.ValueType.TypeIsOptional())
                        {
                            con.GreenLine($"{arg.Description} ({opt.ValueType.GenericTypeArguments[0].Name})");
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
                                con.Yellow(" *REQUIRED");
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

                    var argsArray = commandArgs.ToArray();
                    var displayArgs = argsArray.Select(s => s.Any(char.IsWhiteSpace) ? $"\"{s}\"" : s);
                    con.YellowLine($"Using: gen {gen.GeneratorInfo.ShortName} run {string.Join(" ", displayArgs)}");

                    return command.Invoke(argsArray);

                }

                return command.Invoke(Array.Empty<string>());
            };
        }
    }
}
