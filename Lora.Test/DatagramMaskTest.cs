#region Information

// File DatagramMaskTest.cs has been created by: Dmytro Baranovskyi at:  2023 02 13
// 
// Description:

#endregion

namespace Lora.Test;

[TestClass]
public class DatagramMaskTest
{
    [TestMethod]
    public void ReadMessage_is_data_len()
    {
        ulong data = 0x0000_0002_0000_0000;
        var type = MessageTypeParser.ReadType(data);
        
        Assert.IsTrue(type == ServiceMessageType.DataLength);
    }
    
    [TestMethod]
    public void ReadMessageType_is_ack()
    {
        ulong data = 0x0000_0002_0000_0000;
        var type = MessageTypeParser.ReadType(data);
        
        Assert.IsTrue(type == ServiceMessageType.Ack);
    }
    
    [TestMethod]
    public void ReadMessageType_append()
    {
        ulong data = 0xFF00_0000_0000_0000;
        ulong initial = 0;
        var appended = MessageTypeParser.AppendType(ref initial,ServiceMessageType.Repeat);
        var type = MessageTypeParser.ReadType(appended);
        
        Assert.IsTrue(type == ServiceMessageType.Repeat);
    }

    [TestMethod]
    public void CompleteTest_datagram()
    {
        ulong data = 0L;

        MessageTypeParser.AppendIndex(ref data, 0xAABB);
        MessageTypeParser.AppendType(ref data, ServiceMessageType.Repeat);
        MessageTypeParser.AppendFromAddr(ref data, 0x10);
        MessageTypeParser.AppendToAddr(ref data, 0x20);
        MessageTypeParser.AppendServiceData(ref data, 0xAABBCC00);

        var index = MessageTypeParser.ReadIndex(data);
        Assert.IsTrue(index == 0xAABB);
        var type = MessageTypeParser.ReadType(data);
        Assert.IsTrue(type == ServiceMessageType.Repeat);
        var toAddr = MessageTypeParser.ReadToAddr(data);
        Assert.IsTrue(toAddr == 0x20);
        var from = MessageTypeParser.ReadFromAddr(data);
        Assert.IsTrue(from == 0x10);

        var service = MessageTypeParser.ReadServiceData(data);

        Assert.IsTrue(service == 0x00AABBCC);

    }
}