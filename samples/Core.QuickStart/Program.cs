using System.Text;
using IndustrialComm.Binary;
using IndustrialComm.Checksum;
using IndustrialComm.Diagnostics;
using IndustrialComm.Framing;
using IndustrialComm.Transport;

// IndustrialComm.Core quick start: CRC, binary codec, framing, and stream transport.

Console.WriteLine("IndustrialComm.Core QuickStart");
Console.WriteLine("==============================");

RunSyncDemos();
await RunTransportDemoAsync();

Console.WriteLine();
Console.WriteLine("Done. Next package: IndustrialComm.Modbus (planned).");

static void RunSyncDemos()
{
    // 1) Modbus-style CRC-16
    byte[] pdu = [0x01, 0x03, 0x00, 0x00, 0x00, 0x0A];
    var crc = Crc16.ComputeModbus(pdu);
    Console.WriteLine($"CRC-16/Modbus of request: 0x{crc:X4}");

    // 2) Binary codec
    Span<byte> word = stackalloc byte[2];
    BinaryCodec.WriteUInt16(word, 40001, ByteOrder.BigEndian);
    Console.WriteLine($"40001 as BE bytes: {word[0]:X2} {word[1]:X2}");

    // 3) Frame buffer with fixed-length frames
    var frameBuffer = new FrameBuffer(capacity: 64, detector: new FixedLengthFrameDetector(4));
    frameBuffer.Write([0xDE, 0xAD, 0xBE, 0xEF, 0xCA, 0xFE, 0xBA, 0xBE]);
    Span<byte> frame = stackalloc byte[4];
    while (true)
    {
        var result = frameBuffer.TryReadFrame(frame, out var written);
        if (result.IsFailure)
        {
            Console.WriteLine($"Framing error: {result.Error}");
            break;
        }

        if (written == 0)
        {
            break;
        }

        Console.WriteLine($"Frame: {Convert.ToHexString(frame)}");
    }
}

static async Task RunTransportDemoAsync()
{
    // 4) Stream transport over MemoryStream (stand-in for TCP/serial)
    await using var stream = new MemoryStream();
    var diagnostics = new CountingCommDiagnostics();
    await using var transport = new StreamByteTransport(stream, leaveOpen: true, diagnostics: diagnostics);
    await transport.ConnectAsync();

    var hello = Encoding.ASCII.GetBytes("HELLO");
    await transport.WriteAsync(hello);
    stream.Position = 0;
    var read = new byte[16];
    var n = await transport.ReadAsync(read);
    Console.WriteLine($"Transport echo: {Encoding.ASCII.GetString(read, 0, n)}");
    Console.WriteLine($"Diagnostics: sent={diagnostics.BytesSentTotal}, recv={diagnostics.BytesReceivedTotal}");
}
