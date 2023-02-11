﻿#region Information
// File Datagram.cs has been created by: Dmytro Baranovskyi at:  2023 02 06
// 
// Description:
#endregion

using System.Runtime.InteropServices;

namespace Lora;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct DataFrame
{
    public fixed byte Data[32];
    public uint Crc32;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct Datagram
{
    public uint Id;
    public ushort Destination;
    public DataFrame Frame;
    public uint Crc32;
}