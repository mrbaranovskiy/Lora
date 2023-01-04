using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace MCP.Communication.Misc;

public static class MemExt
{
    public static void CopyTo<T>(this Span<T> span, Memory<T> to, int idx)
    {
        if (span.Length + idx > to.Length)
            throw new IndexOutOfRangeException();

        span.CopyTo(to.Slice(idx).Span);
    }

    public static Memory<T> Pool<T>(this Span<T> span, ArrayPool<T> pool)
    {
        var len = span.Length;
        var rent = pool.Rent(len);
        var mem =  new Memory<T>(rent, 0, len);
        span.CopyTo(mem.Span);

        return mem;
    }

    public static void Release<T>(this Memory<T> memory, ArrayPool<T> pool)
    {
        if (MemoryMarshal.TryGetArray<T>(memory, out var segment))
        {
            if (segment.Array != null) 
                pool.Return(segment.Array);
        }
    }

    public static (byte h, byte l) ToBytes(ushort num)
    {
        var l = (byte)(num & (0xFF));
        var h = (byte)(num >> 8);

        return (h, l);
    }

    public static ushort To16(byte high, byte low)
    {
        return (ushort) ((high << 8) | low);
    }

    public static bool All<T>(this ReadOnlyMemory<T> mem, Func<T, bool> pred)   
    {
        if (mem.IsEmpty) throw new ArgumentException(nameof(mem));
        
        for (int i = 0; i < mem.Length; i++)
            if (!pred(mem.Span[i]))
                return false;

        return true;
    }
}