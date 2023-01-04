using System;
using System.Threading.Tasks;

namespace MCP.Communication.TransportLayer.Transport;

public interface IEncryptionService
{
    void Encrypt(Span<byte> buffer);
    void Decrypt(Span<byte> buffer);
}

public sealed class EncryptedTransport : ITransport<byte>
{
    private IObserver<byte[]> _observer;
    private readonly ITransport<byte> _parentTransport;

    public EncryptedTransport(ITransport<byte> parent)
    {
        _parentTransport = parent;
    }

    public void OnCompleted() => _observer.OnCompleted();

    public void OnError(Exception error) => _observer.OnError(error);

    public void OnNext(byte[] value)
    {
        // decrypt this.
        _observer.OnNext(value);
    }

    public IDisposable Subscribe(IObserver<byte[]> observer)
    {
        _observer = observer;
        return _parentTransport.Subscribe(this);
    }

    public Task WriteAsync(byte[] data)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
    }
}