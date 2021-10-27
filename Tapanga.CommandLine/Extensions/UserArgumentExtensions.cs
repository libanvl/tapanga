using System.CommandLine;
using Tapanga.Plugin;

namespace Tapanga.CommandLine;

internal static class UserArgumentExtensions
{
    public static Option AsOption(this UserArgument arg)
    {
        switch (arg)
        {
            case UserArgument<Uri> uriarg:
                return uriarg.AsOption();
            case UserArgument<string> stringarg:
                return stringarg.AsOption();
            case UserArgument<bool> boolarg:
                return boolarg.AsOption();
            case UserArgument<FileInfo> filearg:
                return filearg.AsOption();
            case UserArgument<DirectoryInfo> directoryArg:
                return directoryArg.AsOption();
        }

        var option = new Option($"--{arg.LongName}", arg.Description)
        {
            IsRequired = arg.Required
        };

        AddOptShortName(arg, option);

        return option;
    }

    public static Option<T> AsOption<T>(this UserArgument<T> arg)
    {
        var option = arg.Default switch
        {
            Opt<T>.Some some => new Option<T>($"--{arg.LongName}", getDefaultValue: some.Unwrap, description: arg.Description)
            {
                IsRequired = arg.Required,
            },

            Opt<T>.None => new Option<T>($"--{arg.LongName}", arg.Description)
            {
                IsRequired = arg.Required
            },

            _ => throw new NotImplementedException()
        };

        if (typeof(T) == typeof(FileInfo))
        {
            option = option.LegalFileNamesOnly();
        }

        if (typeof(T) == typeof(DirectoryInfo))
        {
            option = option.LegalFilePathsOnly();
        }

        AddOptShortName(arg, option);

        return option;
    }

    private static void AddOptShortName(UserArgument arg, Option option)
    {
        if (arg.ShortName is Opt<string>.Some shortname)
        {
            option.AddAlias($"-{shortname.Value}");
        }
    }
}
