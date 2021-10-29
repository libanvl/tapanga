using System.Security.Cryptography;
using System.Text;

namespace Tapanga.Core;

public static class GuidUtilities
{
    public static readonly Guid WindowsTerminalNamespaceGuid = new("2BDE4A90-D05F-401C-9492-E40884EAD1D8");

    public static readonly Guid WindowsTerminalGeneratedNamespaceGuid = new(0xf65ddb7e, 0x706b, 0x4499, 0x8a, 0x50, 0x40, 0x31, 0x3c, 0xaf, 0x51, 0x0a);

    public static Guid NameToGuid(Guid namespaceGuid, string name)
    {
        var namespacedName = EndianSwap(namespaceGuid).ToByteArray().Concat(
            Encoding.Unicode.GetBytes(name)).ToArray();

        byte[] hash = SHA1.HashData(namespacedName);

        byte[] result = new byte[16];

        //Copy first 16-bytes of the hash into our future Guid result
        Array.Copy(hash, result, 16);

        //set high-nibble to 5 to indicate type 5
        result[6] &= 0x0F;
        result[6] |= 0x50;

        //set upper two bits to 2
        result[8] &= 0x3F;
        result[8] |= 0x80;

        return EndianSwap(new Guid(result));
    }

    public static Guid EndianSwap(Guid value)
    {
        var valueBytes = value.ToByteArray().AsSpan();
        var data1 = BitConverter.ToUInt32(valueBytes[..4]);
        var data2 = BitConverter.ToUInt16(valueBytes[4..6]);
        var data3 = BitConverter.ToUInt16(valueBytes[6..8]);
        var data4 = valueBytes[8..];

        data1 = EndianSwap(data1);
        data2 = EndianSwap(data2);
        data3 = EndianSwap(data3);

        return new Guid((Int32)data1, (Int16)data2, (Int16)data3, data4.ToArray());
    }

    private static UInt16 EndianSwap(UInt16 value)
    {
        return (UInt16)((value & 0xFF00) >> 8 |
                (value & 0x00FF) << 8);
    }

    private static UInt32 EndianSwap(UInt32 value)
    {
        return (value & 0xFF000000) >> 24 |
               (value & 0x00FF0000) >> 8 |
               (value & 0x0000FF00) << 8 |
               (value & 0x000000FF) << 24;
    }
}
