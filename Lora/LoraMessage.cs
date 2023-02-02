#region Information

// File LoraMessage.cs has been created by: Dmytro Baranovskyi at:  2023 02 01
// 
// Description:

#endregion

using System.Text.Json.Serialization;
using MCP.Communication.Misc;

public record LoraMessage
{
    [JsonPropertyName("usr")]
    public int User { get; set; }
    public MessageStatus Status { get; set; }
    [JsonPropertyName("body")]
    public string Body { get; set; }
    public byte[] Crc32 { get; set; }
}
