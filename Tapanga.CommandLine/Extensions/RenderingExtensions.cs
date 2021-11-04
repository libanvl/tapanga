using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;

namespace Tapanga.CommandLine;

internal static class RenderingExtensions
{
    public static View AsView(this TextSpan span) =>
        new ContentView(span);

    public static TextSpan Underline(this string value) =>
        new ContentSpan(value).Underline();

    public static TextSpan Underline(this TextSpan value) =>
        new ContainerSpan(StyleSpan.UnderlinedOn(),
                  value,
                  StyleSpan.UnderlinedOff());

    public static TextSpan Bold(this string value) =>
        new ContentSpan(value).Bold();

    public static TextSpan Bold(this TextSpan value) =>
        new ContainerSpan(StyleSpan.BoldOn(),
                  value,
                  StyleSpan.BoldOff());

    public static TextSpan Reverse(this string value) =>
    new ContentSpan(value).Reverse();

    public static TextSpan Reverse(this TextSpan value) =>
        new ContainerSpan(StyleSpan.ReverseOn(),
                  value,
                  StyleSpan.ReverseOff());

    public static TextSpan Rgb(this string value, byte r, byte g, byte b) =>
        new ContainerSpan(ForegroundColorSpan.Rgb(r, g, b),
                          new ContentSpan(value),
                          ForegroundColorSpan.Reset());

    public static TextSpan LightGreen(this string value) =>
        new ContainerSpan(ForegroundColorSpan.LightGreen(),
                          new ContentSpan(value),
                          ForegroundColorSpan.Reset());

    public static TextSpan White(this string value) =>
        new ContainerSpan(ForegroundColorSpan.White(),
                          new ContentSpan(value),
                          ForegroundColorSpan.Reset());

    public static TextSpan Blue(this string value) =>
        new ContainerSpan(ForegroundColorSpan.Blue(),
                      new ContentSpan(value),
                      ForegroundColorSpan.Reset());

    public static TextSpan Cyan(this string value) =>
        new ContainerSpan(ForegroundColorSpan.Cyan(),
                      new ContentSpan(value),
                      ForegroundColorSpan.Reset());

    public static TextSpan Magenta(this string value) =>
        new ContainerSpan(ForegroundColorSpan.Magenta(),
                      new ContentSpan(value),
                      ForegroundColorSpan.Reset());

    public static TextSpan Yellow(this string value) =>
        new ContainerSpan(ForegroundColorSpan.Yellow(),
                      new ContentSpan(value),
                      ForegroundColorSpan.Reset());

    public static TextSpan Red(this string value) =>
        new ContainerSpan(ForegroundColorSpan.Red(),
                      new ContentSpan(value),
                      ForegroundColorSpan.Reset());

    public static TextSpan Green(this string value) =>
        new ContainerSpan(ForegroundColorSpan.Green(),
                  new ContentSpan(value),
                  ForegroundColorSpan.Reset());

    public static OutputMode DetectOutputMode(this IConsole console)
    {
        if (console == null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        if (console is ITerminal terminal &&
            !terminal.IsOutputRedirected)
        {
            return terminal is VirtualTerminal
                       ? OutputMode.Ansi
                       : OutputMode.NonAnsi;
        }
        else
        {
            return OutputMode.PlainText;
        }
    }
}
