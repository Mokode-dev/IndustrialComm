---
title: 帧缓冲与传输
---

# 帧缓冲与传输

## IFrameDetector

| 实现 | 场景 |
|------|------|
| `FixedLengthFrameDetector` | 固定长度报文 |
| `LengthPrefixFrameDetector` | 长度域在帧头（如 MBAP） |
| `DelimiterFrameDetector` | 定界符（如 `:` … CRLF） |

检测结果：`NeedMoreData` / `FrameReady` / `Skip` / `Invalid`。

## FrameBuffer

累计接收字节并抽出完整帧；容量不足返回 `BufferOverflow`。

```csharp
var fb = new FrameBuffer(capacity: 4096, detector);
fb.Write(chunk);
var r = fb.TryReadFrame(); // CommResult<byte[]?>
```

## IByteTransport

协议包应依赖接口，而非具体 Socket/串口类型：

```csharp
public interface IByteTransport : IAsyncDisposable
{
    bool IsConnected { get; }
    ValueTask ConnectAsync(CancellationToken ct = default);
    ValueTask DisconnectAsync(CancellationToken ct = default);
    ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken ct = default);
    ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct = default);
}
```

`StreamByteTransport` 适配任意双向 `Stream`，并接入 `ICommDiagnostics`。
