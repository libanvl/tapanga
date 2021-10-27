using System.Text.Json.Serialization;

namespace Tapanga.Core.Serialization;

internal class FragmentRoot
{
    [JsonPropertyName("tapangaVersion")]
    public TapangaVersion? TapangaVersion { get; set; }

    [JsonPropertyName("profiles")]
    public List<Profile> Profiles { get; set; } = new List<Profile>();
}