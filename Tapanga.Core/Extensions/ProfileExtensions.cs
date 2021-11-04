using Microsoft;
using System.Reflection;
using Tapanga.Plugin;

namespace Tapanga.Core;

public static class ProfileExtensions
{
    public static GeneratorId GetGeneratorId(this IProfileGenerator generator)
    {
        Type generatorType = generator.GetType();
        if (generatorType.GetCustomAttribute<ProfileGeneratorAttribute>(inherit: true) is ProfileGeneratorAttribute attr)
        {
            var assemblyName = generatorType.Assembly.GetName();
            var assemblySimple = assemblyName.Name;
            var assemblyVersion = assemblyName.Version;

            Assumes.NotNull(assemblySimple);
            Assumes.NotNull(assemblyVersion);

            return new GeneratorId(
                key: attr.Key,
                version: attr.Version,
                assemblyName: assemblySimple,
                assemblyVersion: assemblyVersion);
        }

        throw new ArgumentException($"Profile generators must be decorated with {nameof(ProfileGeneratorAttribute)}", nameof(generator));
    }

    internal static Guid GetProfileId(this ProfileData profileData, GeneratorId generatorId)
    {
        var genNamespace = GuidUtilities.NameToGuid(GuidUtilities.WindowsTerminal.GeneratedProfileNamespace, generatorId.GetPluginSerializationName());
        return GuidUtilities.NameToGuid(genNamespace, profileData.Name);
    }
}
