using System.IO.Ports;
using System.Reactive.Linq;
using MCP.Communication.Misc;
using MCP.Communication.TransportLayer;

var portIndex = IndexOfArg(args, "port");

if (portIndex == -1)
{
    Console.Error.WriteLine("Port command is not specified!!");
    return;
}

if (args.Length < 2)
{
    Console.Error.WriteLine("Specify port");
    return;
}

var sp = new SerialPort(args[portIndex + 1], 9600);
var source = Observable.FromEventPattern<SerialDataReceivedEventHandler, SerialDataReceivedEventArgs>(h => sp.DataReceived += h,
    h => sp.DataReceived -= h);
sp.Open();

var _cib = new CirBuffer<byte>(1024);
byte[] ReadSerialData(int s)
{
    var buffer = new byte[s];
    sp.Read(buffer, 0, s);
    return buffer;
}

source
    .Select(_ => sp.BytesToRead)
    .Where(s=> s > 0)
    .Select(ReadSerialData).Subscribe(buffer =>
{
    
    foreach (var b in buffer){ _cib.Push(b); }

    if(_cib.DataLen == 0) return;
    
    try
    {
        var data = _cib.Peek(_cib.DataLen);
        
        var result = HandleMessage(data);

        if (result != null)
            switch (result.Status)
            {
                case MessageStatus.Send:
                {
                    Console.Out.WriteLine($"MSG: {result.Body}");
                    SendData(LoraMessageUtils.AckMessage(0));
                    _cib.RemoveAll();
                    break;
                }
                case MessageStatus.Failed:
                    break;
                case MessageStatus.FailedReceived:
                    Console.Out.WriteLine("Repeat");
                    _cib.RemoveAll();
                    break;
                case MessageStatus.Ack:
                    Console.Out.WriteLine("Message sent");
                    _cib.RemoveAll();
                    break;
                case MessageStatus.Receiving:
                    break;
            }
    }
    catch (Exception)
    {
        Console.WriteLine("general error!");
    }
    
    void SendData(byte[] data) { sp.Write(data, 0, data.Length); }
});

while (true)
{
    Console.Out.WriteLine("Input message:");
    var readLine = Console.ReadLine();

    if (readLine is { } && !string.IsNullOrEmpty(readLine))
    {
        var msg = LoraMessageUtils.SendMessage(readLine, 0);
        sp.Write(msg, 0, msg.Length);
    }
}

LoraMessage? HandleMessage(ReadOnlySpan<byte> data)
{
    if (data == null) throw new ArgumentNullException(nameof(data));
    if (data.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(data));

    var idxStart = data.IndexOfSubsequenceLast(LoraMessageUtils.StartPattern);
    var idxEnd = data.IndexOfSubsequenceLast(LoraMessageUtils.EndPattern);

    if (idxStart != -1 && idxEnd == -1)
        return new LoraMessage {Status = MessageStatus.Receiving};
    if (idxStart != -1 && idxEnd != -1 && idxEnd <= idxStart)
        return new LoraMessage {Status = MessageStatus.Failed};
    if (idxStart == -1 || idxEnd == -1 || idxEnd <= idxStart) 
        return new LoraMessage {Status = MessageStatus.Failed};
    
    var end = idxEnd + (LoraMessageUtils.EndPattern.Length);
    var sp = data[idxStart..end];

    return LoraMessageUtils.DeserializeWrapped(sp);
}


int IndexOfArg(string[] args, string arg)
{
    if (args.Length == 0) return -1;
    
    for (int i = 0; i < arg.Length; i++)
    {
        if (string.Compare(args[i], arg, StringComparison.InvariantCultureIgnoreCase) == 0)
            return i;
    }

    return -1;
}