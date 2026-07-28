---
title: 二进制与校验
---

# 二进制与校验

## BinaryCodec

支持 BE/LE 的 `UInt16/32/64`、`Int*`、`float`、`double`，以及定长 ASCII。

```csharp
BinaryCodec.WriteUInt32(span, value, ByteOrder.BigEndian);
var v = BinaryCodec.ReadSingle(span, ByteOrder.LittleEndian);
```

辅助：`SwapBytes`、`SwapWords`、`ReverseBytes`（适配 PLC 混合字序）。

## BitPacker

Modbus 风格线圈打包（每字节 LSB 为第一位）：

```csharp
BitPacker.PackCoils(coils, destination);
BitPacker.UnpackCoils(source, coils);
```

## BCD

单字节与多字节大端 BCD 编解码，用于仪表与部分 PLC 时间/数值字段。

## 校验算法

| API | 算法 | 典型用途 |
|-----|------|----------|
| `Crc16.ComputeModbus` | CRC-16/Modbus poly 0xA001 | Modbus RTU |
| `Crc16.ComputeCcittFalse` | CRC-16/CCITT-FALSE | 部分现场总线 |
| `Crc32.Compute` | CRC-32 ISO-HDLC | 文件 / 以太网风格 |
| `Lrc.Compute` | 纵向冗余校验 | Modbus ASCII |

已知向量（节选）：

- `"123456789"` → CRC-16/CCITT-FALSE = `0x29B1`  
- `"123456789"` → CRC-32 = `0xCBF43926`  
- RTU 请求 `01 03 00 00 00 0A` → Modbus CRC = `0xCDC5`  
