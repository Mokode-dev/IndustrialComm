using IndustrialComm.Checksum;
using Xunit;

namespace IndustrialComm.Core.Tests;

public class ChecksumTests
{
    [Fact]
    public void Crc16_Modbus_KnownVector()
    {
        // Classic Modbus RTU example: 01 03 00 00 00 0A -> CRC 0xCDC5 (LE on wire: C5 CD)
        byte[] payload = [0x01, 0x03, 0x00, 0x00, 0x00, 0x0A];
        var crc = Crc16.ComputeModbus(payload);
        Assert.Equal(0xCDC5, crc);

        Span<byte> frame = stackalloc byte[8];
        var len = Crc16.AppendModbus(payload, frame);
        Assert.Equal(8, len);
        Assert.Equal(0xC5, frame[6]);
        Assert.Equal(0xCD, frame[7]);
        Assert.True(Crc16.ValidateModbus(frame));
    }

    [Fact]
    public void Crc16_CcittFalse_KnownVector()
    {
        // "123456789" -> 0x29B1 for CRC-16/CCITT-FALSE
        var data = "123456789"u8;
        Assert.Equal(0x29B1, Crc16.ComputeCcittFalse(data));
    }

    [Fact]
    public void Crc32_KnownVector()
    {
        var data = "123456789"u8;
        Assert.Equal(0xCBF43926u, Crc32.Compute(data));
    }

    [Fact]
    public void Lrc_ModbusAsciiStyle()
    {
        // Example from Modbus over serial: 01 03 00 00 00 0A -> LRC 0xF2
        byte[] payload = [0x01, 0x03, 0x00, 0x00, 0x00, 0x0A];
        Assert.Equal(0xF2, Lrc.Compute(payload));
        Assert.True(Lrc.Validate([0x01, 0x03, 0x00, 0x00, 0x00, 0x0A, 0xF2]));
    }
}
