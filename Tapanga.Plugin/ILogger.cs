namespace Tapanga;

/// <summary>
/// Log Levels.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Verbose
    /// </summary>
    Verbose,

    /// <summary>
    /// Information
    /// </summary>
    Information,

    /// <summary>
    /// Warning
    /// </summary>
    Warning,

    /// <summary>
    /// Error
    /// </summary>
    Error
}

/// <summary>
/// Represents a Tapanga Logger
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Add information to the log.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="message"></param>
    void Log(LogLevel logLevel, string message);
}
