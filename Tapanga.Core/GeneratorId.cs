using System.Text.Json.Serialization;

namespace Tapanga.Core;

public record GeneratorId
{
    public GeneratorId(string key, Version version, string assemblyName, Version assemblyVersion)
    {
        Key = key;
        Version = version;
        AssemblyName = assemblyName;
        AssemblyVersion = assemblyVersion;
    }

    public static GeneratorId None { get; } = new GeneratorId(string.Empty, new Version(), string.Empty, new Version());

    [JsonPropertyName("name")]
    public string Key { get; init; }

    [JsonPropertyName("version")]
    public Version Version { get; init; }

    [JsonPropertyName("assemblyName")]
    public string AssemblyName { get; init; }

    [JsonPropertyName("assemblyVersion")]
    public Version AssemblyVersion { get; init; }
}
