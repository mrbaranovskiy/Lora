using System;
using System.IO.Hashing;
using System.Text;
using System.Text.Json;

namespace MCP.Communication.Misc;

public static class LoraMessageUtils
{
    public const string Start = "<m>";
    public const string End = "</m>";

    public static byte[] StartPattern;
    public static byte[] EndPattern;
    
    static LoraMessageUtils()
    {
        StartPattern = Encoding.UTF8.GetBytes(Start);
        EndPattern = Encoding.UTF8.GetBytes(End);
    }

    public static int IndexOfSubsequenceLast(this byte[] source, byte[] pattern)
    {
        if (pattern.Length > source.Length) return -1;
        
        for (var i = source.Length - 1; i >= pattern.Length - 1; i--)
        {
            var found = true;

            for (var j = pattern.Length - 1; j >= 0 && found; j--)
                found = source[i - (pattern.Length - j - 1)] == pattern[j];

            if (found) return i - (pattern.Length - 1);
        }

        return -1;
    }

    public static byte[] AckMessage(int userId)
    {
        return SerializeWrapped(string.Empty, userId, MessageStatus.Ack);
    }
    
    public static byte[] SendMessage(string body, int userId)
    {
        return SerializeWrapped(body, userId, MessageStatus.Send);
    }

    internal static byte[] SerializeMessage(string body, int userid, MessageStatus status)
    {
        var bytes = Encoding.UTF8.GetBytes(body);
        var hash = Crc32.Hash(bytes);

        return JsonSerializer.SerializeToUtf8Bytes(new LoraMessage()
        {
            Status = status,
            User = userid, 
            Body = body,
            Crc32 = hash
        });
    }

    private static byte[] SerializeWrapped(string body, int userId, MessageStatus status)
    {
        if (body == null) throw new ArgumentNullException(nameof(body));
        return WrapMsg(SerializeMessage(body, userId, status));
    }

    public static LoraMessage DeserializeWrapped(byte[] buffer)
    {
        return DeserializeMessage(UnWrapMsg(buffer));
    }
    
    public static LoraMessage DeserializeWrapped(ReadOnlySpan<byte> buffer)
    {
        return DeserializeMessage(UnWrapMsg(buffer));
    }

    public static byte[] WrapMsg(this byte[] buffer)
    {
        var total = buffer.Length + StartPattern.Length + EndPattern.Length;
        var result = new byte[total];

        Array.Copy(StartPattern, 0, result, 0, StartPattern.Length);
        Array.Copy(buffer, 0, result, StartPattern.Length, buffer.Length);
        Array.Copy(EndPattern, 0, result, StartPattern.Length + buffer.Length, EndPattern.Length);

        return result;
    }

    public static LoraMessage DeserializeMessage(ReadOnlySpan<byte> msg) 
        => JsonSerializer.Deserialize<LoraMessage>(msg);

    public static bool IsValidMessage(LoraMessage message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));
        
        var msgCrc = message.Crc32;
        var bytes = Encoding.UTF8.GetBytes(message.Body);
        var calcCrc = Crc32.Hash(bytes);

        return IsEqualArray(msgCrc, calcCrc);
    }

    private static bool IsEqualArray<T>(T[] a, T[] b) where T : IEquatable<T>
    {
        if (a is null && b is null) return false;
        if (a.Length != b.Length) return false;

        return !a.Where((t, i) => t.Equals(b[i])).Any();
    }

    public static ReadOnlySpan<byte> UnWrapMsg(this byte[] data)
    {
        var span = new Span<byte>(data);

        var start = StartPattern.Length;
        var end = EndPattern.Length;

        return span[start..^end];
    }
    
    public static ReadOnlySpan<byte> UnWrapMsg(this ReadOnlySpan<byte> data)
    {

        var start = StartPattern.Length;
        var end = EndPattern.Length;

        return data[start..^end];
    }
}