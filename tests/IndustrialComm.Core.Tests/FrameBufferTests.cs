using IndustrialComm.Framing;
using IndustrialComm.Results;
using Xunit;

namespace IndustrialComm.Core.Tests;

public class FrameBufferTests
{
    [Fact]
    public void FixedLength_ExtractsMultipleFrames()
    {
        var buffer = new FrameBuffer(64, new FixedLengthFrameDetector(4));
        Assert.True(buffer.Write([0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08]).IsSuccess);

        Span<byte> frame = stackalloc byte[4];
        Assert.True(buffer.TryReadFrame(frame, out var n1).IsSuccess);
        Assert.Equal(4, n1);
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04 }, frame.ToArray());

        Assert.True(buffer.TryReadFrame(frame, out var n2).IsSuccess);
        Assert.Equal(4, n2);
        Assert.Equal(new byte[] { 0x05, 0x06, 0x07, 0x08 }, frame.ToArray());
    }

    [Fact]
    public void FixedLength_PartialFrame_NeedsMoreData()
    {
        var buffer = new FrameBuffer(64, new FixedLengthFrameDetector(4));
        Assert.True(buffer.Write([0x01, 0x02]).IsSuccess);

        Span<byte> frame = stackalloc byte[4];
        Assert.True(buffer.TryReadFrame(frame, out var n).IsSuccess);
        Assert.Equal(0, n);
    }

    [Fact]
    public void LengthPrefix_ModbusTcpStyle()
    {
        // MBAP-like: length at offset 4 (2 bytes BE) counts remaining bytes after length field.
        // Total = 6 + length (unit id + PDU).
        var detector = new LengthPrefixFrameDetector(
            lengthFieldOffset: 4,
            lengthFieldSize: 2,
            lengthAdjustment: 6,
            maxFrameLength: 260,
            byteOrder: IndustrialComm.Binary.ByteOrder.BigEndian);

        var buffer = new FrameBuffer(512, detector);
        // Transaction 0, Protocol 0, Length 3, Unit 1, Func 03, ...
        byte[] frame = [0x00, 0x01, 0x00, 0x00, 0x00, 0x03, 0x01, 0x03, 0x00];
        Assert.True(buffer.Write(frame).IsSuccess);

        var result = buffer.TryReadFrame();
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(9, result.Value!.Length);
    }

    [Fact]
    public void Delimiter_SkipsGarbageUntilStart()
    {
        var detector = new DelimiterFrameDetector(
            endDelimiter: "\r\n"u8,
            maxFrameLength: 64,
            startDelimiter: ":"u8);

        var buffer = new FrameBuffer(128, detector);
        Assert.True(buffer.Write("xx:0103\r\n"u8).IsSuccess);

        var result = buffer.TryReadFrame();
        Assert.True(result.IsSuccess);
        Assert.Equal(":0103\r\n", System.Text.Encoding.ASCII.GetString(result.Value!));
    }

    [Fact]
    public void Write_Overflows_ReturnsError()
    {
        var buffer = new FrameBuffer(4, new FixedLengthFrameDetector(2));
        var result = buffer.Write([1, 2, 3, 4, 5]);
        Assert.True(result.IsFailure);
        Assert.Equal(CommErrorCode.BufferOverflow, result.Error!.Code);
    }
}
