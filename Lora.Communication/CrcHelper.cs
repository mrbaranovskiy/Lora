#region Information

// File CrcHelper.cs has been created by: Dmytro Baranovskyi at:  2023 02 06
// 
// Description:

#endregion

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using Aes = System.Runtime.Intrinsics.Arm.Aes;

namespace MCP.Communication.Misc;

public static class CrcHash
{
    public static uint Crc32(ReadOnlySpan<byte> data, uint crcInitial = 0)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (data.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(data));

        var dataLen = data.Length;
        
        if (dataLen >= 8 && dataLen % 8 == 0)
            return Crc32Aligned(data, crcInitial);

        uint crc = crcInitial;
        
        if (Sse42.IsSupported)
        {
            for (int i = 0; i < dataLen; i++)
            {
                crc = Sse42.Crc32(crc, data[i]);
            }

            return crc;
        }

        if(Aes.Arm64.IsSupported)
        {
            var ibStart = 0;
            var cbSize = data.Length - dataLen % 8 ;
            var crc2 = crcInitial;
            
            while (cbSize >= 8)
            {
                crc2 = System.Runtime.Intrinsics.Arm.Crc32.Arm64.ComputeCrc32(crc2, BitConverter.ToUInt64(data.Slice(ibStart, 8)));
                ibStart += 8;
                cbSize -= 8;
            }

            if (dataLen % 8 == 0) return crc2;

            ulong spare = data[dataLen - dataLen % 8];
            
            for (int i = (dataLen - dataLen % 8) + 1; i < dataLen; i++)
            {
                spare = (spare | data[i]) << 8;
            }

            return System.Runtime.Intrinsics.Arm.Crc32.Arm64.ComputeCrc32(crc2, spare);
        }

        throw new InvalidOperationException("crc is not supported on this architecture!!!");
    }
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static uint Crc32Aligned(ReadOnlySpan<byte> data, uint initial = 0)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (data.Length % 8 != 0 || data.Length < 8) throw new ArgumentException("Lang should be multiplied 8");
        
        ulong crc = initial;
        var ibStart = 0;
        var cbSize = data.Length;
        
        if (Sse42.X64.IsSupported)
        {
            while (cbSize >= 4)
            {
                crc = Sse42.X64.Crc32(crc, BitConverter.ToUInt32(data.Slice(ibStart, 4)));
                ibStart += 4;
                cbSize -= 4;
            }

            return (uint)crc;
        }

        var crc2 = initial;

        if (Sse42.IsSupported)
        {
            while (cbSize > 0)
            {
                crc2 = Sse42.Crc32(crc2, data[ibStart]);
                ibStart++;
                cbSize--;
            }

            return crc2;
        }

        if (System.Runtime.Intrinsics.Arm.Crc32.Arm64.IsSupported)
        {
            while (cbSize >= 8)
            {
                crc2 = System.Runtime.Intrinsics.Arm.Crc32.Arm64.ComputeCrc32(crc2, BitConverter.ToUInt64(data.Slice(ibStart, 8)));
                ibStart += 8;
                cbSize -= 8;
            }
        }
        
        throw new InvalidOperationException("crc is not supported on this architecture!!!");
    }
}