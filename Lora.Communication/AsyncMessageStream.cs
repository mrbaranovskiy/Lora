#region Information

// File AsyncMessageStream.cs has been created by: Dmytro Baranovskyi at:  2023 02 12
// 
// Description:

#endregion

using System.Buffers;
using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Lora.Communication.Misc;
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
    private readonly CirBuffer<byte> _cbBuffer = new();
    private readonly ConcurrentQueue<Datagram> _queue = new();
    private readonly int _datagramSize = -1;
    private readonly DatagramBuilder _builder;
    private int _maxQueueCount = 1000;

    public AsyncMessageStream(ITransport<byte> transport)
    {
        _transport = transport;
        _transport.ReadStream(CancellationToken.None)
            .SelectMany(ProcessDatagram)
            .Where(s=> s is {})
            .SubscribeOn(new TaskPoolScheduler(Task.Factory))
            .Subscribe(s =>
            {
                if (_queue.Count < MaxQueueCount)
                {
                    _queue.Enqueue(s);
                }
            });
        
        _datagramSize = Marshal.SizeOf(typeof(Datagram));
        _builder = new DatagramBuilder();
    }

    public int MaxQueueCount
    {
        get => _maxQueueCount;
        set
        {
            if (value is > 0 and < 10000)
            {
                _maxQueueCount = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Range should be 0..10000");
            }
        }
    }

    private Datagram[] ProcessDatagram(byte[] buf)
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
        var frame = new ReadOnlySpan<byte>(dg.Data, LoraConstants.LoraSmallDgLength);
        var crc = CrcHash.Crc32(frame);

        return crc == dg.Crc32 && crc != 0;
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