namespace Tapanga.Plugin;

/// <summary>
/// Describes an assembly as a Tapanga plugin that provides one or more generators.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class PluginAssemblyAttribute : Attribute
{
}
