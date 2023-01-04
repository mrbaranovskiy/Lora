namespace MCP.DeviceDiscovery.Contracts;

internal struct ProfillerHistoryItem
{
    public byte profilerType;
    public uint milliSecondsStart;
    public uint pitTimerStart;
    public uint milliSecondsEnd;
    public uint pitTimerEnd;
}

