using IndustrialComm.Binary;
using Xunit;

namespace IndustrialComm.Core.Tests;

public class BinaryCodecTests
{
    [Fact]
    public void ReadWrite_UInt16_BothEndians()
    {
        Span<byte> buffer = stackalloc byte[2];
        BinaryCodec.WriteUInt16(buffer, 0x1234, ByteOrder.BigEndian);
        Assert.Equal(0x12, buffer[0]);
        Assert.Equal(0x34, buffer[1]);
        Assert.Equal(0x1234, BinaryCodec.ReadUInt16(buffer, ByteOrder.BigEndian));

        BinaryCodec.WriteUInt16(buffer, 0x1234, ByteOrder.LittleEndian);
        Assert.Equal(0x34, buffer[0]);
        Assert.Equal(0x12, buffer[1]);
        Assert.Equal(0x1234, BinaryCodec.ReadUInt16(buffer, ByteOrder.LittleEndian));
    }

    [Fact]
    public void ReadWrite_Single_BigEndian()
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryCodec.WriteSingle(buffer, 1.0f, ByteOrder.BigEndian);
        Assert.Equal(1.0f, BinaryCodec.ReadSingle(buffer, ByteOrder.BigEndian));
    }

    [Fact]
    public void BitPacker_RoundTrip()
    {
        bool[] coils = [true, false, true, true, false, false, false, true, true];
        Span<byte> packed = stackalloc byte[2];
        var written = BitPacker.PackCoils(coils, packed);
        Assert.Equal(2, written);

        var unpacked = new bool[coils.Length];
        BitPacker.UnpackCoils(packed, unpacked);
        Assert.Equal(coils, unpacked);
    }

    [Fact]
    public void Bcd_RoundTrip()
    {
        Assert.Equal(0x25, Bcd.EncodeByte(25));
        Assert.Equal(25, Bcd.DecodeByte(0x25));

        Span<byte> buffer = stackalloc byte[2];
        Bcd.Encode(1234, buffer);
        Assert.Equal(0x12, buffer[0]);
        Assert.Equal(0x34, buffer[1]);
        Assert.Equal(1234UL, Bcd.Decode(buffer));
    }

    [Fact]
    public void Ascii_ReadWrite()
    {
        Span<byte> buffer = stackalloc byte[6];
        BinaryCodec.WriteAscii(buffer, "PLC1");
        Assert.Equal("PLC1", BinaryCodec.ReadAscii(buffer));
    }
}
