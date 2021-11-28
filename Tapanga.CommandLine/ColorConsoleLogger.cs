using Tapanga.Core;

namespace Tapanga.CommandLine;

internal class ColorConsoleLogger : DelegateLogger
{
    public ColorConsoleLogger(ColorConsole console, LogLevel logLevel)
    {
        if (logLevel <= LogLevel.Error)
        {
            this.Delegates[LogLevel.Error] = (_, m) => console.RedLine($"[Error] {m}");
        }

        if (logLevel <= LogLevel.Warning)
        {
            this.Delegates[LogLevel.Warning] = (_, m) => console.YellowLine($"[Warning] {m}");
        }

        if (logLevel <= LogLevel.Information)
        {
            this.Delegates[LogLevel.Information] = (_, m) => console.CyanLine($"[Info] {m}");
        }

        if (logLevel <= LogLevel.Verbose)
        {
            this.Delegates[LogLevel.Verbose] = (_, m) => console.GrayLine($"[Verbose] {m}");
        }
    }
}
