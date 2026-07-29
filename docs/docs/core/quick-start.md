---
title: 快速开始
---

# 快速开始

## 安装

```bash
dotnet add package Mokode.IndustrialComm.Core
```

## CRC 与二进制

```csharp
using IndustrialComm.Binary;
using IndustrialComm.Checksum;

byte[] pdu = [0x01, 0x03, 0x00, 0x00, 0x00, 0x0A];
ushort crc = Crc16.ComputeModbus(pdu);

Span<byte> word = stackalloc byte[2];
BinaryCodec.WriteUInt16(word, 40001, ByteOrder.BigEndian);
```

## 帧缓冲（半包 / 粘包）

```csharp
using IndustrialComm.Framing;

var buffer = new FrameBuffer(256, new FixedLengthFrameDetector(4));
buffer.Write(receivedBytes);

Span<byte> frame = stackalloc byte[4];
var result = buffer.TryReadFrame(frame, out var written);
if (result.IsSuccess && written > 0)
{
    // 处理完整帧
}
```

## 传输适配

任意可读可写 `Stream`（如 `NetworkStream`）可包装为：

```csharp
using IndustrialComm.Transport;

await using var transport = new StreamByteTransport(networkStream);
await transport.ConnectAsync();
await transport.WriteAsync(request);
var n = await transport.ReadAsync(responseBuffer);
```

## 仓库示例

```bash
dotnet run --project samples/Core.QuickStart
```
