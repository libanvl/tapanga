namespace Tapanga.Plugin;

public static class CommandLineFormatExtensions
{
    public static string FormatParameter(this string? v) => string.IsNullOrWhiteSpace(v) ? string.Empty : $" {Utilities.QuoteFormat(v)}";

    public static string FormatParameter(this bool v, string flag) => v ? $" {flag}" : string.Empty;

    public static string FormatParameter<T>(this T v) => v?.ToString().FormatParameter() ?? string.Empty;

    public static string FormatParameter(this Opt<string> v) => v is Opt<string>.Some some ? $" {Utilities.QuoteFormat(some.Value)}" : string.Empty;

    public static string FormatParameter(this Opt<bool> v, string flag) => v is Opt<bool>.Some some && some.Value ? $" {flag}" : string.Empty;

    public static string FormatParameter<T>(this Opt<T> v) => v is Opt<T>.Some some ? some.FormatParameter() : string.Empty;
}