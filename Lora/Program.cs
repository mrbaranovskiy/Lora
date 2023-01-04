using System.IO.Ports;
using System.Reactive.Linq;
using System.Security.AccessControl;
using System.Text;
using MCP.Communication.TransportLayer;
using MCP.Communication.TransportLayer.Transport;

const string Start = "{s}";
const string End = "{e}";
const string Failed = "{f}";
const string Ack = "{ack}";
const string AppendPattern = "{{a}}{0}";

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

source.Subscribe(s =>
{
    var spBytesToRead = sp.BytesToRead;
    if (spBytesToRead <= 0) return;
    
    var buffer = new byte[spBytesToRead];
    sp.Read(buffer, 0, spBytesToRead);
    
    list.AddRange(buffer);

    var result = CheckMessage();

    switch (result.status)
    {
        case MessageStatus.Received:
        {
            Console.Out.WriteLine($"MSG: {result.msg}");
            sp.WriteLine(Ack);
            list.Clear();
            break;
        }
        case MessageStatus.Receiving:      
            break;
        case MessageStatus.Failed:
            // Console.BackgroundColor = ConsoleColor.DarkRed;
            // Console.Out.WriteLine("Failed message");
            // Console.BackgroundColor = ConsoleColor.White;
            break;
        case MessageStatus.FailedReceived:
            Console.Out.WriteLine("Repeat");
            list.Clear();
            break;
        case MessageStatus.AckReceived:
            Console.Out.WriteLine("Message sent");
            list.Clear();
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }

});

while (true)
{
    Console.Out.WriteLine("Input message:");
    var readLine = Console.ReadLine();

    if (readLine is { } && !string.IsNullOrEmpty(readLine))
    {
        sp.WriteLine(WrapMessage(readLine));
    }
}


string WrapMessage(string msg)
{
    return $"{Start}{msg}{End}";
}

(MessageStatus status, string msg) CheckMessage()
{
    var array = list.ToArray();
    var str = Encoding.UTF8.GetString(array);
    var idxStart = str.IndexOf(Start);
    var idxEnd = str.IndexOf(End);

    if (idxStart != -1 && idxEnd == -1)
        return (MessageStatus.Receiving, null!);
    if (idxStart != -1 && idxEnd != -1 && idxEnd <= idxStart)
        return (MessageStatus.Failed, null!);
    if (idxStart != -1 && idxEnd != -1 && idxEnd > idxStart)
    {
        var sub = str.Substring(idxStart + 3, idxEnd - idxStart - 3);
        return (MessageStatus.Received,sub);
    }
    
    if (str.IndexOf(Ack) != -1)
        return (MessageStatus.AckReceived, null!);

    if (str.IndexOf(Failed) != -1)
        return (MessageStatus.FailedReceived, null!);

    return (MessageStatus.Failed, null!);
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

enum MessageStatus
{
    Received,
    Receiving, 
    Failed, 
    FailedReceived, 
    AckReceived,
}