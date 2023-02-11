#region Information

// File DatagramBuilder.cs has been created by: Dmytro Baranovskyi at:  2023 02 10
// 
// Description:

#endregion

using System.Buffers;
using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MCP.Communication.Misc;
using MCP.Communication.TransportLayer;

namespace Lora;

interface IAsyncMessageEnumerator<T> : IAsyncDisposable
{
    ValueTask<bool> MoveNextAsync(CancellationToken token);
    T Current { get; }
}

class AsyncMessageStream : IAsyncMessageEnumerator<Datagram>
{
    private readonly ITransport<byte> _transport;
    private readonly CancellationTokenSource _source;
    private readonly CirBuffer<byte> _cbBuffer = new();
    private readonly ConcurrentQueue<Datagram> _queue = new();
    private readonly int _datagramSize = -1;
    private readonly DatagramBuilder _builder;
    private readonly CirBuffer<byte> _cbSearch = new(64);

    public AsyncMessageStream(ITransport<byte> transport, CancellationTokenSource source)
    {
        _transport = transport;
        _source = source;
        _transport.ReadStream(CancellationToken.None)
            .SelectMany(ProcessDatagram)
            .Where(s=> s is {})
            .SubscribeOn(new TaskPoolScheduler(Task.Factory))
            .Subscribe(s => _queue.Enqueue(s));
        _datagramSize = Marshal.SizeOf(typeof(Datagram));
        _builder = new DatagramBuilder();
    }

    internal Datagram[] ProcessDatagram(byte[] buf)
    {
        _cbBuffer.Push(new Memory<byte>(buf));
        
        if (_cbBuffer.DataLen < _datagramSize || buf is null || !buf.Any()) 
            return null!;
        
        var dgDataChunk = _cbBuffer.Pop(_datagramSize);

        //here we added not valid data.
        var size = dgDataChunk.Length + _cbBuffer.DataLen;
        var pooledBuffer = ArrayPool<byte>.Shared.Rent(size);
        var mem = new Memory<byte>(pooledBuffer, 0, size);

        //copy all to the one buffer
        dgDataChunk.CopyTo(mem);
        var left = _cbBuffer.Pop(_cbBuffer.DataLen);
        left.CopyTo(mem.Slice(dgDataChunk.Length));

        var dgs = new List<Datagram>();        
        
        for (var i = 0; i < size - (_datagramSize - 1); i++)
        {
            var slice = mem.Slice(i, _datagramSize);
            var tempDg = _builder.ReadDatagram(slice.Span);
            var isValid = Validate(tempDg);

            if (!isValid) continue;
            
            dgs.Add(tempDg);
            i += _datagramSize - 1;
        }

        ArrayPool<byte>.Shared.Return(pooledBuffer);

        return dgs.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private unsafe bool Validate(Datagram dg)
    {
        var frame = new ReadOnlySpan<byte>(dg.Frame.Data, 32);
        var crc = CrcHash.Crc32(frame);

        return crc == dg.Frame.Crc32 && crc != 0;
    }

    public async ValueTask DisposeAsync()
    {
        await _transport.DisposeAsync();
    }

    public async ValueTask<bool> MoveNextAsync(CancellationToken token)
    {
        int i = 0;
        while (!token.IsCancellationRequested)
        {
            if (i >= 20)
                return false;
            
            if (!_queue.TryDequeue(out var result))
            {
                await Task.Yield();
                i++;
                continue;
            }

            Current = result;
            return true;
        }

        return false;
    }

    public Datagram Current { get; private set; }
}

internal interface ITransport<T> : IAsyncDisposable
{
    Task Write(T[] data);
    Task Write(ReadOnlySpan<T> data);
    
    IObservable<T[]> ReadStream(CancellationToken token);
}

internal interface IDataSplitter
{
    IEnumerable<Datagram> SplitData(Stream dataStream);
    IEnumerable<Datagram> SplitData(ReadOnlySpan<byte> dataStream);
}

public class DatagramBuilder
{
    private readonly ArrayPool<byte> _pool;

    public DatagramBuilder(ArrayPool<byte> pool)
    {
        _pool = pool ?? throw new ArgumentNullException(nameof(pool));
    }

    public DatagramBuilder()
    {
        _pool = ArrayPool<byte>.Shared;
    }

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

    internal unsafe ReadOnlyMemory<byte> BuildDatagram(uint id, ushort to, ReadOnlySpan<byte> data)
    {
        Memory<byte> res = new byte[Marshal.SizeOf(typeof(Datagram))];
        var crcFrame = CrcHash.Crc32Aligned(data);
        var dg = new Datagram { };

        for (int i = 0; i < data.Length; i++)
            dg.Frame.Data[i] = data[i];

        dg.Frame.Crc32 = crcFrame;
        dg.Id = id;
        dg.Destination = to;
        
        Marshal.StructureToPtr(dg, (nint)res.Pin().Pointer, true);
        
        var crcDg = CrcHash.Crc32(res.Span[..^4]);

        var bytes = BitConverter.GetBytes(crcDg);
        bytes.CopyTo(res.Span[^4..]);

        return res;
    }
}