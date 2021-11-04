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

    public string Key { get; init; }

    public Version Version { get; init; }

    public string AssemblyName { get; init; }

    public Version AssemblyVersion { get; init; }
}
