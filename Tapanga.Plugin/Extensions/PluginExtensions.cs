using libanvl;
using System.Reflection;

namespace Tapanga.Plugin;

/// <summary>
/// Extension methods for creating plugins
/// </summary>
public static class PluginExtensions
{
    /// <summary>
    /// Creates a <see cref="StreamIcon"/> from the <paramref name="iconResourceName"/> contained in <paramref name="assembly"/>.
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="iconResourceName"></param>
    /// <returns>An <see cref="Opt{StreamIcon}.Some"/> if the resource exists, an <see cref="Opt{Icon}.None"/> is the resource does not exist.</returns>
    public static Opt<Icon> GetStreamIcon(this Assembly assembly, string iconResourceName)
    {
        var resource = assembly.GetManifestResourceStream(iconResourceName);

        Opt<Icon> icon = resource is null
            ? Opt<Icon>.None
            : Opt.Some<Icon>(new StreamIcon(iconResourceName, resource));

        return icon;
    }
}
