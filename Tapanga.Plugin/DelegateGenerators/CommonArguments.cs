namespace Tapanga.Plugin;

/// <summary>
/// Common user arguments for Windows Terminal profiles.
/// </summary>
public class CommonArguments
{
    /// <summary>
    /// The display name of the profile.
    /// </summary>
    [UserArgument("Profile: name", ShortName = "pn", IsRequired = true, Sort = -302)]
    [DefaultValueFactory(nameof(GetDefaultProfileName))]
    public string? ProfileName { get; set; }

    /// <summary>
    /// The display title of new tabs created with this profile.
    /// </summary>
    [UserArgument("Profile: tab title", ShortName = "pt", Sort = -301)]
    public string? ProfileTitle { get; set; }

    /// <summary>
    /// The starting directory for the profile.
    /// </summary>
    [UserArgument("Profile: starting directory", ShortName = "pd", Sort = -300)]
    [DefaultValueFactory(nameof(GetDefaultStartingDirectory))]
    public DirectoryInfo? StartingDirectory { get; set; }

    /// <summary>
    /// Get a default profile name.
    /// </summary>
    public static string GetDefaultProfileName() => $"tapanga profile {Utilities.GetShortRandomId()}";

    /// <summary>
    /// Get the default starting directory.
    /// </summary>
    public static DirectoryInfo GetDefaultStartingDirectory()
    {
        return new DirectoryInfo(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
    }
}
