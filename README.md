# IndustrialComm

生产级 .NET 工业通信中间件生态。统一传输、二进制、校验、帧缓冲、结果模型和诊断抽象，协议实现可插拔。

| 包 | 状态 | 说明 |
|----|------|------|
| **IndustrialComm.Core** | ✅ 1.0.0 | 生态地基 |
| IndustrialComm.Modbus | 规划中 | Modbus RTU/ASCII/TCP |
| IndustrialComm.Devices | 规划中 | 设备 / 点位抽象 |
| IndustrialComm.Hosting | 规划中 | DI / 后台采集 Host |

文档站点（Docusaurus）：见 [`docs/`](docs/)。

## 快速开始

```bash
dotnet add package IndustrialComm.Core
```

```csharp
using IndustrialComm.Checksum;
using IndustrialComm.Binary;

byte[] pdu = [0x01, 0x03, 0x00, 0x00, 0x00, 0x0A];
ushort crc = Crc16.ComputeModbus(pdu);

Span<byte> buffer = stackalloc byte[2];
BinaryCodec.WriteUInt16(buffer, 40001, ByteOrder.BigEndian);
```

运行示例：

```bash
dotnet run --project samples/Core.QuickStart
```

## 构建与测试

```bash
dotnet build IndustrialComm.sln -c Release
dotnet test IndustrialComm.sln -c Release
dotnet pack src/IndustrialComm.Core/IndustrialComm.Core.csproj -c Release -o artifacts
```

## 设计原则

- **可预期失败用 `CommResult`**，编程错误才抛异常
- **协议无关传输** `IByteTransport`，TCP / 串口 / 测试双端可替换
- **Span 友好** 的热路径，减少分配
- **低依赖**：`netstandard2.0;net8.0`

## 许可证

[MIT](LICENSE)
