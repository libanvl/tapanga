namespace Tapanga.Plugin;

/// <summary>
/// Describes a Terminal Profile.
/// </summary>
/// <param name="Name">The profile name</param>
/// <param name="CommandLine">The profile command line</param>
/// <param name="StartingDirectory">An optional starting directory</param>
/// <param name="TabTitle">An option tab title</param>
/// <param name="Icon">An optional icon</param>
public record ProfileData(
    string Name,
    string CommandLine,
    Opt<DirectoryInfo> StartingDirectory,
    Opt<string> TabTitle,
    Opt<Icon> Icon);