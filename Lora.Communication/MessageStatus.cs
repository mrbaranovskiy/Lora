using Aes = System.Runtime.Intrinsics.Arm.Aes;

namespace MCP.Communication.Misc;

public enum MessageStatus
{
    Send,
    Received,
    Receiving, 
    Failed, 
    FailedReceived, 
    Ack,
}