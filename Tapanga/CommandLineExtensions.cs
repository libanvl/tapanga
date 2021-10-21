using System.CommandLine;
using Tapanga.Plugin;

namespace Tapanga.CommandLine;

internal static class CommandLineExtensions
{
    public static T WithAlias<T>(this T command, params string[] aliases) where T : Command
    {
        foreach (var a in aliases)
        {
            command.AddAlias(a);
        }

        return command;
    }

    public static bool TypeIsOptional(this Type valueType)
    {
        return valueType.IsGenericType && valueType.IsAssignableTo(typeof(OptionalArgument<>).MakeGenericType(valueType.GenericTypeArguments));
    }
}
