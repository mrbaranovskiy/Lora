using System.Text;
using MCP.Communication.Misc;

static class MessageUtils
{
    public static string DecodeMessage(byte[] buffer)
    {
        return buffer.Length < 6 ? string.Empty : Encoding.ASCII.GetString(buffer[3..^2]);
    }

    public static byte[] EncodeMessage(string buffer)
    {
        var data = Encoding.ASCII.GetBytes(buffer);
        var bytes = new byte[data.Length + 5];
        data.CopyTo(bytes,3);

        var hl = MemExt.ToBytes((ushort) (bytes.Length));

        bytes[0] = 0xAA;
        bytes[2] = hl.h;
        bytes[1] = hl.l;

        var crc16 = MemExt.ToBytes(Crc.Crc16(new ReadOnlySpan<byte>(data)));


        bytes[^1] = crc16.h;
        bytes[^2] = crc16.l;

        return bytes;
    }
}