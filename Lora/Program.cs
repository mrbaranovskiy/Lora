using System.IO.Ports;
using System.Reactive.Linq;
using MCP.Communication.Misc;

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

var list = new List<byte>();

byte[] ReadSerialData(int s)
{
    var buffer = new byte[s];
    sp.Read(buffer, 0, s);
    return buffer;
}

void SendData(byte[] data)
{
    sp.Write(data, 0, data.Length);
}

source
    .Select(_ => sp.BytesToRead)
    .Where(s=> s > 0)
    .Select(ReadSerialData).Subscribe(buffer =>
{
    
    list.AddRange(buffer);

    try
    {
        var result = HandleMessage();

        switch (result.Status)
        {
            case MessageStatus.Send:
            {
                Console.Out.WriteLine($"MSG: {result.MessageBody}");
                //Send the ack response
                SendData(LoraMessageUtils.AckMessage(0));
                list.Clear();
                break;
            }
            case MessageStatus.Failed:
                break;
            case MessageStatus.FailedReceived:
                Console.Out.WriteLine("Repeat");
                list.Clear();
                break;
            case MessageStatus.Ack:
                Console.Out.WriteLine("Message sent");
                list.Clear();
                break;
            case MessageStatus.Receiving:
                break;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("general error!");
    }
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

LoraMessage HandleMessage()
{
    var raw = list.ToArray();
    
    var idxStart = raw.IndexOfSubsequenceLast(LoraMessageUtils.StartPattern);
    var idxEnd = raw.IndexOfSubsequenceLast(LoraMessageUtils.EndPattern);

    if (idxStart != -1 && idxEnd == -1)
        return new LoraMessage {Status = MessageStatus.Receiving};
    if (idxStart != -1 && idxEnd != -1 && idxEnd <= idxStart)
        return new LoraMessage {Status = MessageStatus.Failed};
    if (idxStart == -1 || idxEnd == -1 || idxEnd <= idxStart) 
        return new LoraMessage() {Status = MessageStatus.Failed};
    
    var end = idxEnd + (LoraMessageUtils.EndPattern.Length );
    var sp = raw.AsSpan(idxStart..end);

    return LoraMessageUtils.DeserializeWrapped(sp);
}


int IndexOfArg(string[] args, string arg)
{
    for (int i = 0; i < arg.Length; i++)
    {
        if (string.Compare(args[i], arg, StringComparison.InvariantCultureIgnoreCase) == 0)
            return i;
    }

    return -1;
}
