#region Information

// File IDataSplitter.cs has been created by: Dmytro Baranovskyi at:  2023 02 12
// 
// Description:

#endregion

using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Lora.Communication.Misc;

namespace Lora;

// internal interface IDataSplitter
// {
//     IEnumerable<byte[]> SplitData(Stream dataStream, in LoraAddress destination);
//     IEnumerable<byte[]> SplitData(ReadOnlySpan<byte> dataStream, in LoraAddress destination);
// }

[StructLayout(LayoutKind.Sequential)]
internal struct LoraAddress
{
    public ushort To;
}

internal class SynchronousDataSplitter : IEnumerator<ReadOnlyMemory<byte>>, IEnumerable<ReadOnlyMemory<byte>>
{
    private readonly DatagramBuilder _builder;
    private ReadOnlyMemory<byte> _data;
    private uint _currentIndex = 0;
    private readonly int _dgSize;
    private readonly ArrayPool<byte> _pool;
    private byte[] _lastDataChunk;

    private readonly ReaderWriterLockSlim _slimLock = new();

    public SynchronousDataSplitter(DatagramBuilder builder, byte[] buffer)
    {
        if (buffer == null) throw new ArgumentNullException(nameof(buffer));
        if (buffer.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(buffer));
        _builder = builder;
        _pool = ArrayPool<byte>.Shared;
        _lastDataChunk = _pool.Rent(4);

        InitData(buffer);

        _dgSize = Marshal.SizeOf<Datagram>();
    }

    private void InitData(byte[] buffer)
    {
        _data = new ReadOnlyMemory<byte>(buffer);
    }

    public bool MoveNext()
    {
        try
        {
            _slimLock.EnterReadLock();
            _pool.Return(_lastDataChunk);

            var start = _dgSize * (int) _currentIndex;
            var dgLen = _dgSize;
            var dataLen = LoraConstants.LoraSmallDgLength;

            if (start >= _data.Length)
                return false;

            if (_data.Length - start < dataLen)
                dataLen = _data.Length - start;

            var data = _data.Slice(start, dataLen);

            _lastDataChunk = _builder.BuildDatagram(_currentIndex, 0, data.Span, _pool);
            Current = new ReadOnlyMemory<byte>(_lastDataChunk, 0, dgLen);

            _currentIndex++;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            _slimLock.ExitReadLock();
        }

        return true;
    }

    public void Reset()
    {
        _currentIndex = 0;
        _pool.Return(_lastDataChunk);
    }

    public ReadOnlyMemory<byte> Current { get; private set; }

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        Reset();
    }

    public IEnumerator<ReadOnlyMemory<byte>> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}