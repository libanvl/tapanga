using System.Collections;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Collections;
using System.CommandLine.Parsing;
using System.CommandLine.Suggestions;
using Tapanga.Core;
using Tapanga.Plugin;

namespace Tapanga.CommandLine;

internal class OptionAdapter : IOption
{
    private readonly UserArgument _userArgument;
    private readonly Option _option;

    public OptionAdapter(UserArgument userArgument)
    {
        _userArgument = userArgument;
        _option = GetOption(userArgument);
        BoundType = GetBoundType(_option);
    }

    public static implicit operator Option(OptionAdapter adapter) => adapter._option;

    public static implicit operator UserArgument(OptionAdapter adapter) => adapter._userArgument;

    public Type BoundType { get; }

    public string LongName => _userArgument.LongName;

    public IArgument Argument => ((IOption)_option).Argument;

    public bool IsRequired => _option.IsRequired;

    public bool AllowMultipleArgumentsPerToken => _option.AllowMultipleArgumentsPerToken;

    public IReadOnlyCollection<string> Aliases => _option.Aliases;

    public string Name => _option.Name;

    public string? Description => _option.Description;

    public bool IsHidden => _option.IsHidden;

    public ISymbolSet Children => _option.Children;

    public ISymbolSet Parents => _option.Parents;

    public string ValueName => ((IValueDescriptor)_option).ValueName;

    public Type ValueType => ((IValueDescriptor)_option).ValueType;

    public bool HasDefaultValue => ((IValueDescriptor)_option).HasDefaultValue;

    public IArgumentArity Arity => _option.Arity;

    object? IValueDescriptor.GetDefaultValue()
    {
        return this.GetDefaultValue().SomeOrDefault(default!);
    }

    public string GetDefaultValuesString()
    {
        var defaultValue = GetDefaultValue();

        if (defaultValue is Opt<object>.Some some)
        {
            if (some.Value is string someString)
            {
                return someString;
            }

            if (some.Value is IEnumerable enumerable)
            {
                string outputValue = string.Empty;
                foreach (var o in enumerable)
                {
                    outputValue += $" {o}";
                }

                return outputValue.Trim();
            }

            return some.Value.ToString()!;
        }

        return string.Empty;
    }

    public Opt<object> GetDefaultValue()
    {
        return ((IValueDescriptor)_option).GetDefaultValue().WrapOpt();
    }

    public IEnumerable<string> GetSuggestions(ParseResult? parseResult = null, string? textToMatch = null)
    {
        return ((ISuggestionSource)_option).GetSuggestions(parseResult, textToMatch);
    }

    public bool HasAlias(string alias)
    {
        return ((IIdentifierSymbol)_option).HasAlias(alias);
    }

    private static Option GetOption(UserArgument arg)
    {
        var option = new Option($"--{arg.LongName}", arg.Description, arg.ValueType)
        {
            IsRequired = arg.Required
        };

        if (arg.ShortName is Opt<string>.Some shortname)
        {
            option.AddAlias($"-{shortname.Value}");
        }

        // defer this to command generation?
        if (arg.DefaultObject is not null)
        {
            option.SetDefaultValue(arg.DefaultObject);
        }

        if (option.ValueType.IsBoundType(typeof(FileInfo)))
        {
            option = option.LegalFileNamesOnly();
        }

        if (option.ValueType.IsBoundType(typeof(DirectoryInfo)))
        {
            option = option.LegalFilePathsOnly();
        }

        return option;
    }

    private static Type GetBoundType(Option opt)
    {
        if (opt.ValueType.IsAssignableToOpenGeneric(typeof(IEnumerable<>)))
        {
            return opt.ValueType.GenericTypeArguments[0];
        }

        return opt.ValueType;
    }
}
