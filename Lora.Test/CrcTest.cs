#region Information
// File CrcTest.cs has been created by: Dmytro Baranovskyi at:  2023 02 06
// 
// Description:
#endregion

using System.Globalization;
using System.Reactive.Subjects;
using MCP.Communication.Misc;

namespace Lora.Test;

[TestClass]
public class CrcTest
{
    [TestMethod]
    public void TestCrcCalculation()
    {
        unsafe
        {
            byte[] testData = new byte[32];
            new Random().NextBytes(testData);

            var builder = new DatagramBuilder();
            var dg = builder.BuildDatagram(42,42, testData);
            var back = builder.ReadDatagram(dg.ToArray());
            Span<byte> sp = new Span<byte>(back.Frame.Data, 32);
            var initialStructureBytes = builder.BuildDatagram(back.Id, back.Destination, sp);
            var back2 = builder.ReadDatagram(initialStructureBytes.ToArray());

            Assert.IsTrue(Equals(back, back2));
        }
    }

    [TestMethod]
    public async Task TestCollectDataGram_should_be_two()
    {
        byte[] testData = new byte[32];
        new Random().NextBytes(testData);
        var builder = new DatagramBuilder();
        var a = builder.BuildDatagram(42,42, testData);
        var b = builder.BuildDatagram(42,43, testData); 
        var c = builder.BuildDatagram(44,42, testData);

        var total = new Memory<byte>(new byte[a.Length + b.Length + c.Length]);
        
        a.CopyTo(total);
        b.CopyTo(total.Slice(a.Length));
        c.CopyTo(total.Slice(a.Length + b.Length));
        
        var transport = new TransportMock();

        var msgStream = new AsyncMessageStream(transport, new CancellationTokenSource());
        transport.Emit(total.ToArray());
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(1000));
        
        var result = await WaitForSignal(msgStream, 
            static (_, i) => i == 3,
            cts.Token);
        
        Assert.IsTrue(result);

    }

    private async Task<bool> WaitForSignal(IAsyncMessageEnumerator<Datagram> msgStream, Func<Datagram, int, bool> condition, CancellationToken token)
    {
        int count = 0;

        while (!token.IsCancellationRequested)
        {
            if (await msgStream.MoveNextAsync(CancellationToken.None))
            {
                count++;

                if (condition(msgStream.Current, count))
                {
                    return true;
                }
            }

            await Task.Yield();
        }

        return false;
    }
}


public class TransportMock : ITransport<byte>
{
    private BehaviorSubject<byte[]> _emmiter;
    public TransportMock()
    {
        _emmiter = new BehaviorSubject<byte[]>(null);
    }
    
    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public Task Write(byte[] data)
    {
        throw new NotImplementedException();
    }

    public Task Write(ReadOnlySpan<byte> data)
    {
        throw new NotImplementedException();
    }


    public void Emit(byte[] data) => _emmiter.OnNext(data);

    public IObservable<byte[]> ReadStream(CancellationToken token)
    {
        return _emmiter;
    }
}