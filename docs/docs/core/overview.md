---
title: Core 概览
---

# IndustrialComm.Core

生态的第一个生产级 NuGet 包，为所有协议与应用层提供统一地基。

## 模块

| 命名空间 | 内容 |
|----------|------|
| `IndustrialComm.Binary` | 端序读写、Bit 打包、BCD、ASCII 定长串 |
| `IndustrialComm.Checksum` | CRC-16/Modbus、CRC-16/CCITT-FALSE、CRC-32、LRC |
| `IndustrialComm.Framing` | `FrameBuffer`、固定长度 / 长度前缀 / 定界符检测 |
| `IndustrialComm.Transport` | `IByteTransport`、`StreamByteTransport` |
| `IndustrialComm.Results` | `CommResult` / `CommResult<T>`、`CommError`、`CommErrorCode` |
| `IndustrialComm.Reliability` | `RetryPolicy`、`TimeoutGate` |
| `IndustrialComm.Diagnostics` | `ICommDiagnostics`、Null / Counting 实现 |
| `IndustrialComm.Primitives` | `DataQuality`、`CommTimestamp`、`EngineeringUnit` |
| `IndustrialComm.Options` | `CommOptions` 基类 |

## 目标框架

- `netstandard2.0`（工控存量）  
- `net8.0`（现代运行时，含 AOT 友好标记）  

## 安装

```bash
dotnet add package IndustrialComm.Core
```

## 不在 Core 中的内容

完整 Modbus/OPC UA、设备点位模型、DI Host、日志库绑定——这些由后续包提供，避免 Core 膨胀。
