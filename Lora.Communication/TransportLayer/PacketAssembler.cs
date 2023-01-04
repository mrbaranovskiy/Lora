using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace MCP.Communication.TransportLayer;

public sealed class PacketAssembler : ITransport<byte>
{
    private IObserver<byte[]>? _observer;
    private readonly ITransport<byte> _transport;
    private const byte PacketStart = 0xAA;
    private const int MaxDelay = 500;
    private readonly CirBuffer<byte> _cb;

    public PacketAssembler(ITransport<byte> transport)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));

        _cb = new CirBuffer<byte>(1024 * 1024);
        _transport
            .Synchronize()
            .Subscribe((s) =>
            {
                AssemblePacket(s);
            }, exception => OnError(exception));
    }

    public async Task WriteAsync(byte[] data)
    {
        if (!disposedValue)
            await _transport.WriteAsync(data);
        else OnCompleted();
    }

    private bool _packetInProgress;
    private int _incomingDataLen = -1;
    private DateTime _last = DateTime.Now;
    private bool disposedValue;

    private void AssemblePacket(ReadOnlyMemory<byte> incommingData)
    {
        var empty = _cb.DataLen == 0;
        bool isCompleted = false;

        if (_packetInProgress)
        {
            int diff = (int) (DateTime.UtcNow - _last).TotalMilliseconds;
            
            if (diff > MaxDelay)
            {
                _packetInProgress = true;

                // return if no new packet start incomming.
                if (FindStart(incommingData) == -1)
                    return;

                _last = DateTime.UtcNow;
                _incomingDataLen = -1;
                _cb.RemoveAll();
            }
        }
        else
        {
            _last = DateTime.UtcNow;
        }
        
        if (empty || _packetInProgress)
        {
            if (_packetInProgress && FindStart() == -1)
            {
                //packet is in progress but FSR there is not start for the packet.
                _cb.Remove(_cb.DataLen);
                _cb.Push(incommingData);
            }

            if (!empty && FindStart() != -1)
            {
                _cb.Push(incommingData);
            }
            else
            {
                var idx = FindStart(incommingData);
                if (idx == -1) return;

                var packetStart = incommingData.Span[idx..];
                _cb.Push(packetStart);
            }

            _packetInProgress = true;
        }

        if (_cb.DataLen >= 3 && _incomingDataLen == -1)
        {
            _incomingDataLen = ReadPacketLen();
            // start + len + type + crc == 6 bytes 
            if (_incomingDataLen < 6) return;
        }

        if (_incomingDataLen <= _cb.DataLen)
        {
            var result = _cb.Pop(_incomingDataLen);
            //LogExt.Verbose($"Rise on message received {LogExt.ToString(result)}");
            OnNext(result);
            
            var nextStartPos = FindStart();
            if (nextStartPos >= 0)
            {
                //Packet has been received. We assume that some trash, may be at the 
                //end if the prev packet
                _cb.Remove(nextStartPos);

                // check the packes in the sequence. 
                var nextPacketLen = ReadPacketLen();
                if (nextPacketLen != -1)
                {
                    _incomingDataLen = nextPacketLen;
                    _packetInProgress = false;
                    AssemblePacket(_cb.Pop(_cb.DataLen));
                }
            }
            else if (_cb.DataLen > 0)
            {
                // trash left. remote it.
                _cb.Remove(_cb.DataLen);
            }

            isCompleted = true;
            _packetInProgress = _cb.DataLen > 0;
        }

        if (isCompleted)
        {
            _incomingDataLen = -1;
            _packetInProgress = _cb.DataLen != 0;
        }
    }

    private int FindStart()
    {
        for (int i = 0; i < _cb.DataLen; i++)
        {
            if (_cb.PeekItem(i) == PacketStart)
                return i;
        }

        return -1;
    }

    private int FindStart(ReadOnlyMemory<byte> memory)
    {
        for (int i = 0; i < memory.Length; i++)
        {
            if (memory.Span[i] == PacketStart)
                return i;
        }

        return -1;
    }

    private int ReadPacketLen()
    {
        if (_cb.DataLen < 3)
            throw new InvalidOperationException("Not enought data to read len.");

        var high = _cb.PeekItem(1);
        var low = _cb.PeekItem(2);
        var len = low << 8 | high;

        return len;
    }


    //public void Dispose()
    //{
    //    _transport.Dispose();
    //    _cb.Dispose();
    //}

    public void OnCompleted() => _observer?.OnCompleted();
    public void OnError(Exception error) => _observer?.OnError(error);
    public void OnNext(byte[] value) => _observer?.OnNext(value);

    public IDisposable Subscribe(IObserver<byte[]> observer)
    {
        _observer = observer;
        return this;
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _transport.Dispose();
                _cb.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~packetassembler()
    // {
    //     // do not change this code. put cleanup code in 'dispose(bool disposing)' method
    //     dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}