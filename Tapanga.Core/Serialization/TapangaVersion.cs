using System.Text.Json.Serialization;

namespace Tapanga.Core.Serialization;

internal class TapangaVersion
{
    public TapangaVersion(Version pluginVersion, Version coreVersion)
    {
        PluginVersion = pluginVersion;
        CoreVersion = coreVersion;
    }

    [JsonPropertyName("pluginVersion")]
    public Version PluginVersion { get; init; }

    [JsonPropertyName("coreVersion")]
    public Version CoreVersion { get; init; }
}