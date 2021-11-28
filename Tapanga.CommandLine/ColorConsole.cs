using System.CommandLine.IO;
using System.CommandLine.Rendering;
using System.Diagnostics.CodeAnalysis;

namespace Tapanga.CommandLine;

internal class ColorConsole : ITerminal
{
    private readonly ITerminal _terminal;

    public ConsoleColor BackgroundColor { get => _terminal.BackgroundColor; set => _terminal.BackgroundColor = value; }

    public ConsoleColor ForegroundColor { get => _terminal.ForegroundColor; set => _terminal.ForegroundColor = value; }

    public int CursorLeft { get => _terminal.CursorLeft; set => _terminal.CursorLeft = value; }

    public int CursorTop { get => _terminal.CursorTop; set => _terminal.CursorTop = value; }

    public IStandardStreamWriter Out => _terminal.Out;

    public bool IsOutputRedirected => _terminal.IsOutputRedirected;

    public IStandardStreamWriter Error => _terminal.Error;

    public bool IsErrorRedirected => _terminal.IsErrorRedirected;

    public bool IsInputRedirected => _terminal.IsInputRedirected;

    public ColorConsole(ITerminal terminal)
    {
        _terminal = terminal;
    }

    public void Write<T>(T? value, ConsoleColor fg = default, ConsoleColor bg = default)
    {
        BackgroundColor = bg;
        ForegroundColor = fg;
        Out.Write($"{value}");
        ResetColor();
    }

    public void WriteLine<T>(T? value, ConsoleColor fg = default, ConsoleColor bg = default)
    {
        Write($"{value}{Environment.NewLine}", fg, bg);
    }

    public void WriteLine() => Write("\n");

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

    public void ResetColor()
    {
        _terminal.ResetColor();
    }

    public void Clear()
    {
        _terminal.Clear();
    }

    public void SetCursorPosition(int left, int top)
    {
        _terminal.SetCursorPosition(left, top);
    }

    public void HideCursor()
    {
        _terminal.HideCursor();
    }

    public void ShowCursor()
    {
        _terminal.ShowCursor();
    }
}
