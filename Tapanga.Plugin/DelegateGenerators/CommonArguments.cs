using CrypticWizard.RandomWordGenerator;
using static CrypticWizard.RandomWordGenerator.WordGenerator;

namespace Tapanga.Plugin;

public class CommonArguments : IGeneratorArguments
{
    private static readonly List<PartOfSpeech> namePattern = new List<PartOfSpeech>
    {
        PartOfSpeech.adv,
        PartOfSpeech.adj,
        PartOfSpeech.noun
    };

    public CommonArguments(
        string profileName,
        OptionalArgument<string> profileTitle,
        OptionalArgument<DirectoryInfo> startingDirectory)
    {
        ProfileName = profileName;
        ProfileTitle = profileTitle;
        StartingDirectory = startingDirectory;
    }

    [UserArgument("Profile: name", ShortName = "pn", Sort = -302)]
    [DefaultValueFactory(nameof(GetDefaultProfileName))]
    public string ProfileName { get; }

    [UserArgument("Profile: tab title", ShortName = "pt", Sort = -301)]
    public OptionalArgument<string> ProfileTitle { get; }

    [UserArgument("Profile: starting directory", ShortName = "pd", Sort = -300)]
    [DefaultValueFactory(nameof(GetDefaultStartingDirectory))]
    public OptionalArgument<DirectoryInfo> StartingDirectory { get; }

    public static string GetDefaultProfileName()
    {
        WordGenerator myWordGenerator = new WordGenerator();
        return myWordGenerator.GetPattern(namePattern, ' ');
    }

    public static DirectoryInfo GetDefaultStartingDirectory()
    {
        return new DirectoryInfo(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
    }
}
