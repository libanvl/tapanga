using System.Diagnostics.CodeAnalysis;

namespace Tapanga.CommandLine;

internal class ColorConsole
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
    public void WriteLine() => Console.WriteLine();

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
    public void WriteLine<T>(T? value, ConsoleColor fg = default, ConsoleColor bg = default)
    {
        Console.BackgroundColor = bg;
        Console.ForegroundColor = fg;
        Console.WriteLine(value);
        Console.ResetColor();
    }

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
    public void Write<T>(T? value, ConsoleColor fg = default, ConsoleColor bg = default)
    {
        Console.BackgroundColor = bg;
        Console.ForegroundColor = fg;
        Console.Write(value);
        Console.ResetColor();
    }

    public void Reset() => Console.ResetColor();

    public void Blue<T>(T? value, ConsoleColor bg = default) =>
        Write(value, ConsoleColor.Blue, bg);

    public void BlueLine<T>(T? value, ConsoleColor bg = default) =>
        WriteLine(value, ConsoleColor.Blue, bg);

    public void Cyan<T>(T? value, ConsoleColor bg = default) =>
        Write(value, ConsoleColor.Cyan, bg);

    public void CyanLine<T>(T? value, ConsoleColor bg = default) =>
        WriteLine(value, ConsoleColor.Cyan, bg);

    public void DarkCyan<T>(T? value, ConsoleColor bg = default) =>
        Write(value, ConsoleColor.DarkCyan, bg);

    public void DarkCyanLine<T>(T? value, ConsoleColor bg = default) =>
        WriteLine(value, ConsoleColor.DarkCyan, bg);

    public void DarkBlue<T>(T? value, ConsoleColor bg = default) =>
        Write(value, ConsoleColor.DarkBlue, bg);

    public void DarkBlueLine<T>(T? value, ConsoleColor bg = default) =>
        WriteLine(value, ConsoleColor.DarkBlue, bg);

    public void Gray<T>(T? value, ConsoleColor bg = default) =>
        Write(value, ConsoleColor.Gray, bg);

    public void GrayLine<T>(T? value, ConsoleColor bg = default) =>
        WriteLine(value, ConsoleColor.Gray, bg);

    public void Green<T>(T? value, ConsoleColor bg = default) =>
        Write(value, ConsoleColor.Green, bg);

    public void GreenLine<T>(T? value, ConsoleColor bg = default) =>
        WriteLine(value, ConsoleColor.Green, bg);

    public void Magenta<T>(T? value, ConsoleColor bg = default) =>
        Write(value, ConsoleColor.Magenta, bg);

    public void MagentaLine<T>(T? value, ConsoleColor bg = default) =>
        WriteLine(value, ConsoleColor.Magenta, bg);

    public void Red<T>(T? value, ConsoleColor bg = default) =>
        Write(value, ConsoleColor.Red, bg);

    public void RedLine<T>(T? value, ConsoleColor bg = default) =>
        WriteLine(value, ConsoleColor.Red, bg);

    public void Yellow<T>(T? value, ConsoleColor bg = default) =>
        Write(value, ConsoleColor.Yellow, bg);

    public void YellowLine<T>(T? value, ConsoleColor bg = default) =>
        WriteLine(value, ConsoleColor.Yellow, bg);
}
