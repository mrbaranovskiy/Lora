using System;
using System.Runtime.CompilerServices;

namespace MCP.Communication.Misc;

public static class Crc
{
    static ushort[] oddparity = { 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0 };
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Crc16(ReadOnlySpan<byte> data) => Crc16(0, data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Crc16(ushort crc16In, ReadOnlySpan<byte> p)
    {
        ushort data;
       
        for (int i = 0; i < p.Length; i++)
        {
            data = (ushort) ((p[i] ^ (crc16In & 0xff)) & 0xff);
            crc16In >>= 8;

            if ((oddparity[data & 0x0f] ^ oddparity[data >> 4]) != 0)
                crc16In ^= 0xc001;

            data <<= 6;
            crc16In ^= data;
            data <<= 1;
            crc16In ^= data;
        }

        return crc16In;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Crc16(IntPtr ptr, int len)
    {
        if (ptr == IntPtr.Zero || len <= 0) 
            return 0;
            
        unsafe
        {
            Span<byte> data = new Span<byte>((void*) ptr, len);
            return Crc16(0, data);
        }
    }
    
    public static ushort ReadCrc(Memory<byte> msg)
    {
        var mem = msg.Slice(msg.Length - 2, 2).Span;
        return (ushort) (mem[1] << 8 | mem[0]);
    }
}