namespace Tapanga.Plugin;

public static class CommandLineFormatExtensions
{
    public static string FormatParameter(this string v) => string.IsNullOrWhiteSpace(v) ? string.Empty : $" {v}";

    public static string FormatParameter(this bool v, string flag) => v ? $" {flag}" : string.Empty;

    public static string FormatParameter(this Opt<string> v) => v is Opt<string>.Some some ? $" {some.Value}" : string.Empty;

    public static string FormatParameter(this Opt<bool> v, string flag) => v is Opt<bool>.Some some && some.Value ? $" {flag}" : string.Empty;

    public static string FormatParameter(this OptionalArgument<string> v) => v.AsOpt().FormatParameter();

    public static string FormatParameter(this OptionalArgument<bool> v, string flag) => v.AsOpt().FormatParameter(flag);
}