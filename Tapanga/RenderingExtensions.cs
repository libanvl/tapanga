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
}
