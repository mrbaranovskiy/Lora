// See https://aka.ms/new-console-template for more information


using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Lora;
using MCP.Communication.Misc;

BenchmarkDotNet.Running.BenchmarkRunner.Run<CrcBenchmark>();


[SimpleJob(RuntimeMoniker.Net70, baseline: true)]
[MemoryDiagnoser()]
[RPlotExporter]
public class CrcBenchmark
{
    private byte[] _data;

    [GlobalSetup]
    public void Setup()
    {
        _data = new byte[N];
        new Random(42).NextBytes(_data);
        var builder = new DatagramBuilder();
        _initialStructureBytes = builder.BuildDatagram(42, 42, _data.AsSpan());
        _builder = builder;
    }

    [Params(32)] public int N;
    private ReadOnlyMemory<byte> _initialStructureBytes;
    private DatagramBuilder _builder;
   
    [Benchmark]
    public object VectorizedCrc32_array()
    {
        return CrcHash.Crc32Aligned(_data);
    }
    
}