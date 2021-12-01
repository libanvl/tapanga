namespace Tapanga.CommandLine;

/// <summary>
/// Known command-line exit codes.
/// </summary>
public static class Errors
{
    /// <summary>
    /// An unknown error.
    /// </summary>
    public const int UnknownError = -100;

    /// <summary>
    /// An error during serialization operations.
    /// </summary>
    public const int SerializationError = -101;

    /// <summary>
    /// An error during a profile operation.
    /// </summary>
    public const int ProfileCommandError = -102;
}
