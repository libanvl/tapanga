using Microsoft;
using System.Reflection;
using Tapanga.Plugin;

namespace Tapanga.Core;

public static class TypeExtensions
{
    public static Opt<ConstructorInfo> GetGenericWrapperConstructor(this Type wrapperType, Type boundType)
    {
        return GetGenericConstructor(wrapperType, boundType, boundType);
    }

    public static Opt<ConstructorInfo> GetGenericConstructor(this Type genericType, Type boundType, params Type[] constructorTypes)
    {
        if (!genericType.IsGenericType)
        {
            return Opt.None<ConstructorInfo>();
        }

        var boundGenericType = genericType.MakeGenericType(boundType);
        if (boundGenericType is null)
        {
            return Opt.None<ConstructorInfo>();
        }

        return boundGenericType.GetConstructor(constructorTypes).WrapOpt();
    }

    public static bool IsAssignableToOpenGeneric(this Type derived, Type @base)
    {
        if (derived.IsGenericType)
        {
            var closedGeneric = @base.MakeGenericType(derived.GetGenericArguments());
            return derived.IsAssignableTo(closedGeneric);
        }

        return false;
    }

    public static bool IsBoundType(this Type type, Type boundType)
    {
        if (type.IsAssignableToOpenGeneric(typeof(IEnumerable<>)))
        {
            return type.GenericTypeArguments[0] == boundType;
        }

        return type == boundType;
    }

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

    internal static Guid GetProfileId(this ProfileData profileData)
    {
        Guid namespaceGuid = GuidUtilities.NameToGuid(GuidUtilities.WindowsTerminalGeneratedNamespaceGuid, "Tapanga");
        return GuidUtilities.NameToGuid(namespaceGuid, profileData.Name);
    }
}
