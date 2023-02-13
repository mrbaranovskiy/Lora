#region Information

// File DatagramBuilder.cs has been created by: Dmytro Baranovskyi at:  2023 02 10
// 
// Description:

#endregion

using System.Buffers;
using System.Runtime.InteropServices;
using Lora.Communication.Misc;
using MCP.Communication.Misc;

namespace Lora;

internal interface ITransport<T> : IAsyncDisposable
{
    Task Write(T[] data);
    Task Write(ReadOnlySpan<T> data);
    
    IObservable<T[]> ReadStream(CancellationToken token);
}


internal sealed class DatagramBuilder
{
    internal unsafe Datagram ReadDatagram(byte[] data)
    {
        Memory<byte> res = data;
        using var pin = res.Pin();
        return Marshal.PtrToStructure<Datagram>((nint) pin.Pointer);
    }

    internal Datagram ReadDatagram(ReadOnlySpan<byte> data)
    {
        return MemoryMarshal.Read<Datagram>(data);
    }

    internal byte[] BuildDatagram(uint id, ushort to, ReadOnlySpan<byte> data)
    {
        return BuildDatagram(id, to, data, ArrayPool<byte>.Shared);
    }

    private ReadOnlySpan<byte> EnsureSize(ReadOnlySpan<byte> data)
    {
        if (data.Length % 32 != 0)
        {
            var buf = new Span<byte>(new byte[LoraConstants.LoraSmallDgLength]);
            
            data.CopyTo(buf);
            return buf;
        }

        return data;
    }

    /// <summary>
    /// Builds the datagram as byte buffer.
    /// </summary>
    /// <param name="id">Id of the datagram</param>
    /// <param name="to">Destination.</param>
    /// <param name="data">Data content</param>
    /// <param name="pool">Pool to store data.</param>
    /// <returns>Bytes buffer with stored <see cref="Datagram"/></returns>
    /// <exception cref="ArgumentException"></exception>
    internal unsafe byte[] BuildDatagram(uint id, ushort to, ReadOnlySpan<byte> data, ArrayPool<byte> pool)
    {
        data = EnsureSize(data);
        
        var arr = pool.Rent(Marshal.SizeOf(typeof(Datagram)));
        
        Memory<byte> res = arr;
        var crcFrame = CrcHash.Crc32(data);
        var dg = new Datagram();

        for (int i = 0; i < data.Length; i++)
            dg.Data[i] = data[i];

        dg.Crc32 = crcFrame;
        // dg.Id = id;
        // dg.Destination = to;
        
        Marshal.StructureToPtr(dg, (nint)res.Pin().Pointer, true);
        
        var crcDg = CrcHash.Crc32(res.Span[..^4]);

        var bytes = BitConverter.GetBytes(crcDg);
        bytes.CopyTo(res.Span[^4..]);

        return arr;
    }
}