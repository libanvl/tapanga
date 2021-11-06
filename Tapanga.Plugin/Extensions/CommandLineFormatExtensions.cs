using libanvl;

namespace Tapanga.Plugin;

public static class CommandLineFormatExtensions
{
    public static string Format(this string? v) => string.IsNullOrWhiteSpace(v) ? string.Empty : $" {Utilities.QuoteFormat(v)}";

    public static string Format(this bool v, string flag) => v ? $" {flag}" : string.Empty;

    public static string Format<T>(this T v) => v?.ToString().Format() ?? string.Empty;

    public static string Format(this Opt<string> v) => v is Opt<string>.Some some ? $" {Utilities.QuoteFormat(some.Value)}" : string.Empty;

    public static string Format(this Opt<bool> v, string flag) => v is Opt<bool>.Some some && some.Value ? $" {flag}" : string.Empty;

    public static string Format<T>(this Opt<T> v) => v is Opt<T>.Some some ? some.Format() : string.Empty;
}