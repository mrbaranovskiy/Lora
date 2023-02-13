#region Information
// File Utils.cs has been created by: Dmytro Baranovskyi at:  2023 02 13
// 
// Description:
#endregion

using MCP.Communication.Misc;

namespace Lora.Communication.Misc;

internal static class Utils
{
    internal static unsafe bool IsValid(this Datagram dg)
    {
        var frame = new ReadOnlySpan<byte>(dg.Data, 32);
        var crc = CrcHash.Crc32(frame);

        return crc == dg.Crc32 && crc != 0;
    }
}

internal class LoraConstants
{
    public const int LoraSmallDgLength = 32;
    public const int LoraLargeDgLength = 128;
}