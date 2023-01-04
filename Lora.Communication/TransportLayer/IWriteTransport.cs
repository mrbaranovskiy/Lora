using System.Threading.Tasks;

namespace MCP.Communication.TransportLayer;

public interface IWriteTransport<T> where T : struct
{
    Task WriteAsync(byte[] data);
}