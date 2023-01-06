using System.Text.Json.Serialization;

namespace MCP.Communication.Misc;

public record LoraMessage
{
    [JsonPropertyName("usr")]
    public int User { get; set; }
    public MessageStatus Status { get; set; }
    [JsonPropertyName("body")]
    public string MessageBody { get; set; }
}