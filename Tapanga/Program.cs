using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Tapanga.Core;
using Tapanga.Plugin;

namespace Tapanga.CommandLine;

public class Program
{
    internal const string RootDescription = "TaPaN-Ga: Terminal Profile N-Generator";

    public static async Task<int> Main(string[] args)
    {
        try
        {
            var profiles = new ProfileCollection();

            int ret = await BuildCommandLine(profiles)
                .UseDefaults()
                .UseMiddleware(BuildServiceMiddleware)
                .Build()
                .InvokeAsync(args);

            if (ret != 0)
            {
                return ret;
            }

            foreach (var p in profiles)
            {
                Console.WriteLine($"PROFILE: {p}");
            }

            return 0;
        }
        finally
        {
            Console.ResetColor();
        }
    }

    private static Task BuildServiceMiddleware(InvocationContext context, Func<InvocationContext, Task> next)
    {
        context.BindingContext.AddService<ColorConsole>(_ => new());
        return next(context);
    }

    private static CommandLineBuilder BuildCommandLine(ProfileCollection profiles)
    {
        var gm = new GeneratorManager();

        var generatorCommand = new Command("generator")
        {
            Description = "Interact with the available generators"
        };

        foreach (var pg in gm.GetProfileGenerators())
        {
            IProfileGenerator generator = pg switch
            {
                IDelegateProfileGenerator delgen => new DelegateGeneratorAdapter(delgen),
                _ => pg
            };

            var runCommand = new Command("run")
            {
                Handler = CommandHandler.Create(generator.GetGeneratorDelegate(profiles)),
                Description = $"Run the {generator.GeneratorInfo.LongName} generator using only command-line parameters"
            }
            .WithAlias("r");

            if (generator is IProvideUserArguments argProvider)
            {
                foreach (var userArg in argProvider.GetUserArguments())
                {
                    runCommand.AddOption(userArg.AsOption());
                }
            }

            var genCommand = new Command(generator.GeneratorInfo.ShortName, generator.GeneratorInfo.LongName)
            {
                new Command("info")
                {
                    Handler = CommandHandler.Create(generator.GetInfoHandler()),
                    Description = $"Get extra information about the {generator.GeneratorInfo.LongName} generator"
                }
                .WithAlias("i"),

                new Command("go")
                {
                    Handler = CommandHandler.Create(generator.GetGoHandler(profiles)),
                    Description = $"Start the {generator.GeneratorInfo.LongName} generator in interactive mode"
                },

                runCommand
            };

            generatorCommand.Add(genCommand);
        }

        return new CommandLineBuilder(
            new RootCommand(RootDescription)
            {
                generatorCommand.WithAlias("gen", "g")
            });
    }
}