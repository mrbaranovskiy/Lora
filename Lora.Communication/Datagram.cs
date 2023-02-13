#region Information
// File Datagram.cs has been created by: Dmytro Baranovskyi at:  2023 02 06
// 
// Description:
#endregion

using System.Numerics;
using System.Runtime.InteropServices;
using Lora.Communication.Misc;

namespace Lora;


/// <summary>
/// Service information
/// 0-15    : - packet index
/// 16-23   : - to addr
/// 24-31   : - from addr
/// 32-39   : - message type (ack, data len, etc)
/// 40-63   : - service data, depends on message type. 
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct Datagram
{
    public ulong ServiceInformation;
    public fixed byte Data[LoraConstants.LoraSmallDgLength];
    public uint Crc32;
}

internal class DatagramConstants
{
    public const ulong DatagramPacketIndexMask = 0x0000_0000_0000_FFFF;
    public const ulong DatagramToAddrMask =      0x0000_0000_00FF_0000;
    public const ulong DatagramFromAddrMask =    0x0000_0000_FF00_0000;
    public const ulong DatagramTypeMask =        0x0000_00FF_0000_0000;
    public const ulong DatagramServiceDataMask = 0xFFFF_FF00_0000_0000;
}

internal enum ServiceMessageType : byte
{
    Invalid = 0b0000_0000,
    Ack = 0b0000_0001, 
    Repeat = 0b0000_0011,
    DataLength = 0b0000_0010,
    
    Data = 0b1111_0000,
}
