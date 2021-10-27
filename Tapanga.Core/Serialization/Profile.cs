using System.Text.Json.Serialization;

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

    [JsonPropertyName("tapangeMetadata")]
    public TapangaMetadata TapangaMetadata { get; set; } = TapangaMetadata.None;
}

