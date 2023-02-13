#region Information

// File MessageTypeParser.cs has been created by: Dmytro Baranovskyi at:  2023 02 13
// 
// Description:
// Utilities to read datagram data.

#endregion

using System.Runtime.CompilerServices;

namespace Lora;

internal static class MessageTypeParser
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong AppendType(ref ulong initial, ServiceMessageType type)
        => DoShift(ref initial, (ulong) type, 32);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong AppendIndex(ref ulong initial, ushort index)
        => DoShift(ref initial, index, 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong AppendToAddr(ref ulong initial, byte addr)
        => DoShift(ref initial, addr, 16);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong AppendFromAddr(ref ulong initial, byte addr)
        => DoShift(ref initial, addr, 24);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong AppendServiceData(ref ulong initial, uint serviceData)
    {
        var clearData = (serviceData & 0xFFFF_FF00) >> 8;
        return initial |= (ulong)clearData << 40;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong DoShift(ref ulong initial, ulong data, int shift)
        => initial |= data << shift;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadIndex(in ulong data)
    {
        var index = data & DatagramConstants.DatagramPacketIndexMask;
        return (ushort) (index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ReadToAddr(in ulong data)
    {
        var addr = data & DatagramConstants.DatagramToAddrMask;

        return (byte) (addr >> 16);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ReadFromAddr(in ulong data)
    {
        var addr = data & DatagramConstants.DatagramFromAddrMask;

        return (byte) (addr >> 24);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ServiceMessageType ReadType(in ulong data)
    {
        var type = data & DatagramConstants.DatagramTypeMask;
        return (ServiceMessageType) (byte) (type >> 32);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ReadServiceData(in ulong data)
    {
        return (uint) ((data & DatagramConstants.DatagramServiceDataMask) >> 40);
    }
}