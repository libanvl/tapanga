namespace Tapanga.Core;

public class DelegateLogger : ILogger
{
    public Dictionary<LogLevel, Action<LogLevel, string>> Delegates { get; } = new();

    public void Log(LogLevel logLevel, string message)
    {
        if (Delegates.TryGetValue(logLevel, out var logAction))
        {
            logAction(logLevel, message);
        }
    }
}
