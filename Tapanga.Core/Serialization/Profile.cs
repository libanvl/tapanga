using System.Text.Json.Serialization;
using Tapanga.Plugin;

namespace Tapanga.Core.Serialization;

// FragmentRoot myDeserializedClass = JsonSerializer.Deserialize<FragmentRoot>(myJsonResponse);
internal class Profile
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("commandline")]
    public string? Commandline { get; set; }

    [JsonPropertyName("tabTitle")]
    public string? TabTitle { get; set; }

    [JsonPropertyName("startingDirectory")]
    public string? StartingDirectory { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("tapangaMetadata")]
    public TapangaMetadata TapangaMetadata { get; set; } = TapangaMetadata.None;
}

internal static class ProfileExtensions
{
    public static ProfileData AsProfileData(this Profile profile)
    {
        return new ProfileData(
            profile.Name!,
            profile.Commandline!,
            string.IsNullOrWhiteSpace(profile.StartingDirectory) ? Opt.None<DirectoryInfo>() : new DirectoryInfo(profile.StartingDirectory),
            profile.TabTitle!,
            string.IsNullOrWhiteSpace(profile.Icon) ? Opt.None<Icon>() : new PathIcon(Path.GetFileName(profile.Icon), profile.Icon)
        );
    }
}

