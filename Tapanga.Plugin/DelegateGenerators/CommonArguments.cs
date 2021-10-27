namespace Tapanga.Plugin;

public class CommonArguments : IGeneratorArguments
{
    [UserArgument("Profile: name", ShortName = "pn", IsRequired = true, Sort = -302)]
    [DefaultValueFactory(nameof(GetDefaultProfileName))]
    public string? ProfileName { get; set; }

    [UserArgument("Profile: tab title", ShortName = "pt", Sort = -301)]
    public string? ProfileTitle { get; set; }

    [UserArgument("Profile: starting directory", ShortName = "pd", Sort = -300)]
    [DefaultValueFactory(nameof(GetDefaultStartingDirectory))]
    public DirectoryInfo? StartingDirectory { get; set; }

    public static string GetDefaultProfileName()
    {
        return $"tapanga profile {Utilities.GetShortRandomId()}";
    }

    public static DirectoryInfo GetDefaultStartingDirectory()
    {
        return new DirectoryInfo(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
    }
}
