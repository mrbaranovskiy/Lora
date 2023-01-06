using System.Text;
using MCP.Communication.Misc;

namespace Lora.Test;

[TestClass]
public class MessageUtilsTest
{
    [TestMethod]
    public void CheckDeserialization()
    {
        var msg = LoraMessageUtils.SerializeMessage("data", 42, MessageStatus.Ack);

        var deserialize = LoraMessageUtils.DeserializeMessage(msg);

        Assert.IsTrue(deserialize.User == 42);
        Assert.IsTrue(deserialize.MessageBody == "data");
    }
    
    [TestMethod]
    public void CheckWrap()
    {
        var msg = LoraMessageUtils.SerializeMessage("data", 42, MessageStatus.Ack);

        var wrapMsg = msg.WrapMsg();
        var unWrap = wrapMsg.UnWrapMsg();
        
        var deserialize = LoraMessageUtils.DeserializeMessage(unWrap);
        
        Assert.IsTrue(deserialize.User == 42);
        Assert.IsTrue(deserialize.MessageBody == "data");
    }

    [TestMethod]
    public void CheckSubsequence()
    {
        var idx = new byte[] {1, 2, 3, 4, 5}.IndexOfSubsequenceLast(new byte[] {3,4,5});
        Assert.IsTrue(idx == 2);
    }
    
    [TestMethod]
    public void CheckSubsequenceNoFound()
    {
        var idx = new byte[] {1, 2, 3, 4}.IndexOfSubsequenceLast(new byte[] {3,4,5});
        Assert.IsTrue(idx == -1);
    }
    
    [TestMethod]
    public void CheckSubsequence2()
    {
        var idx = new byte[] {3,4,5}.IndexOfSubsequenceLast(new byte[] {3,4,5});
        Assert.IsTrue(idx == 0);
    }
    
    
    [TestMethod]
    public void CheckSubsequence3()
    {
        var idx = new byte[] {3,4,5,2,3,5,6,3}.IndexOfSubsequenceLast(new byte[] {3,4,5});
        Assert.IsTrue(idx == 0);
    }
    
    [TestMethod]
    public void CheckSubsequence4()
    {
        var idx = new byte[] {3,4,5,2,3,3,4,5}.IndexOfSubsequenceLast(new byte[] {3,4,5});
        Assert.IsTrue(idx == 5);
    }
    
    [TestMethod]
    public void CheckSubsequence5()
    {
        var idx = new byte[] {3,4}.IndexOfSubsequenceLast(new byte[] {3,4,5});
        Assert.IsTrue(idx == -1);
    }
}