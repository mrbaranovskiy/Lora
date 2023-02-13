#region Information

// File DataSplitterTest.cs has been created by: Dmytro Baranovskyi at:  2023 02 13
// 
// Description:

#endregion

using Lora.Communication.Misc;

namespace Lora.Test;

[TestClass]
public class DataSplitterTest
{
    [TestMethod]
    public void SplitData()
    {
        var builder = new DatagramBuilder();
        const int len = 1024;
        var data = new byte[len];
        new Random().NextBytes(data);
        
        using var splitter = new SynchronousDataSplitter(builder, data);

        var all = splitter.Select(s => builder.ReadDatagram(s.ToArray())).Select(s=>s.IsValid()).All(s=>s);
        Assert.IsTrue(all, "all datagrams are valid.");
    }
}