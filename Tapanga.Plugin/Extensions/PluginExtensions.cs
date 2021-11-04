using libanvl;
using System.Reflection;

namespace Tapanga.Plugin;

public static class PluginExtensions
{
    public static Opt<Icon> GetOptIcon(this Assembly assembly, string iconResourceName)
    {
        var resource = assembly.GetManifestResourceStream(iconResourceName);

        Opt<Icon> icon = resource is null
            ? Opt.None<Icon>()
            : Opt.Some<Icon>(new StreamIcon(iconResourceName, resource));
        return icon;
    }
}
