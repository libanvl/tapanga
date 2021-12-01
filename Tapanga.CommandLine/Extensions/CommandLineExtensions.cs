using System.CommandLine;
using System.CommandLine.Builder;

namespace Tapanga.CommandLine;

internal static class CommandLineExtensions
{
    public static CommandLineBuilder BuildServiceMiddleware(this CommandLineBuilder builder)
    {
        return builder.UseMiddleware(context =>
        {
            context.BindingContext.AddService(_ => new ColorConsole(context.Console));
        });
    }

    public static T WithAlias<T>(this T command, params string[] aliases) where T : Command
    {
        foreach (var a in aliases)
        {
            command.AddAlias(a);
        }

        return command;
    }

    public static Option<T> WithAlias<T>(this Option<T>  option, params string[] aliases)
    {
        foreach (var a in aliases)
        {
            option.AddAlias(a);
        }

        return option;
    }
}
