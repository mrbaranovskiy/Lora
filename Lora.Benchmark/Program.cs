// See https://aka.ms/new-console-template for more information


using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MCP.Communication.Misc;

BenchmarkDotNet.Running.BenchmarkRunner.Run<CrcBenchmark>();


[SimpleJob(RuntimeMoniker.Net70, baseline: true)]
[RPlotExporter]
public class CrcBenchmark
{
    private byte[] _data;

    [GlobalSetup]
    public void Setup()
    {
        _data = new byte[N];
        new Random(42).NextBytes(_data);
    }

    [Params(32, 512)] public int N;


    [Benchmark()]
    public object FmaCrc()
    {
        ulong crc = 0;
        var ibStart = 0;
        var cbSize = N;

        while (cbSize >= 4)
        {
            crc = Fma.X64.Crc32(crc, BitConverter.ToUInt32(_data, ibStart));
            ibStart += 4;
            cbSize -= 4;
        }

        return (uint) crc;
    }

    [Benchmark()]
    public object Sse42Crc()
    {
        ulong crc = 0;
        var ibStart = 0;
        var cbSize = N;

        while (cbSize >= 4)
        {
            crc = Sse42.X64.Crc32(crc, BitConverter.ToUInt32(_data, ibStart));
            ibStart += 4;
            cbSize -= 4;
        }

        return (uint) crc;
    }
    
}