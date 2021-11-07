using libanvl;

namespace Tapanga.Plugin;

/// <summary>
/// Extension methods for formatting parameters in command lines.
/// </summary>
public static class CommandLineFormatExtensions
{
    /// <summary>
    /// Returns a quoted string with a prepended space if <paramref name="v"/> is not null or empty.
    /// </summary>
    /// <param name="v"></param>
    public static string Format(this string? v) => string.IsNullOrWhiteSpace(v) ? string.Empty : $" {Utilities.QuoteFormat(v)}";

    /// <summary>
    /// Returns <paramref name="flag"/> prepended with a space if <paramref name="v"/> is <c>true</c>.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="flag"></param>
    public static string Format(this bool v, string flag) => v ? $" {flag}" : string.Empty;

    /// <summary>
    /// Returns a stringified form of <paramref name="v"/>, and runs it through <see cref="Format(string?)"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="v"></param>
    /// <returns></returns>
    public static string Format<T>(this T v) => v?.ToString().Format() ?? string.Empty;

    /// <summary>
    /// Returns a quoted string with a prepended space if <paramref name="v"/> is not <see cref="Opt{String}.None"/>.
    /// </summary>
    /// <param name="v"></param>
    public static string Format(this Opt<string> v) => v is Opt<string>.Some some ? $" {Utilities.QuoteFormat(some.Value)}" : string.Empty;

    /// <summary>
    /// Returns <paramref name="flag"/> prepended with a space if <paramref name = "v" /> is <see cref="Opt{Boolean}.Some"/> and <c>true</c>.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static string Format(this Opt<bool> v, string flag) => v is Opt<bool>.Some some && some.Value ? $" {flag}" : string.Empty;

    /// <summary>
    /// Returns a stringified form of <paramref name="v"/>, and runs it through <see cref="Format(string?)"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="v"></param>
    /// <returns></returns>
    public static string Format<T>(this Opt<T> v) => v is Opt<T>.Some some ? some.Format() : string.Empty;
}