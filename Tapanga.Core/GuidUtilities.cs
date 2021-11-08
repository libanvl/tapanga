using libanvl;

namespace Tapanga.Core;

public static class GuidUtilities
{
    public static class WindowsTerminal
    {
        public static readonly Guid Namespace = new("2bde4a90-d05f-401c-9492-e40884ead1d8");

        public static readonly Guid GeneratedProfileNamespace = new("f65ddb7e-706b-4499-8a50-40313caf510a");
    }

    public static Guid NameToGuid(Guid namespaceGuid, string name) => UUID.V(namespaceGuid, name);
}
