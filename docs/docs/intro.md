---
sidebar_position: 1
title: 生态概览
---

# IndustrialComm

**IndustrialComm** 是面向工业现场与边缘侧的 .NET 通信中间件生态：协议可插拔，统一设备与诊断模型，配套生产级文档与示例。

## 分层架构

```
L3  应用/场景   Gateway · Scada · AspNetCore · Hosting
L2  协议实现    Modbus · OPC UA · MQTT · S7 · …
L1  设备模型    Devices（点位 / 读写 / 轮询）
L0  核心地基    IndustrialComm.Core  ★ 已发布 1.0
```

| 层级 | 包 | 职责 |
|------|-----|------|
| L0 | `IndustrialComm.Core` | 二进制、校验、帧缓冲、传输抽象、Result、重试/超时、诊断 |
| L1 | `IndustrialComm.Devices`（规划） | 设备与点位统一 API |
| L2 | `IndustrialComm.Modbus` 等（规划） | 具体协议 |
| L3 | Hosting / AspNetCore（规划） | 托管、健康检查、管理 API |

## 设计原则

1. **可预期失败用 `CommResult`**，编程错误才抛异常  
2. **传输与协议解耦**（`IByteTransport`）  
3. **热路径 Span 友好**，控制分配  
4. **低依赖**：`netstandard2.0` + `net8.0`  
5. **语义化版本**，公开 API 变更写入 CHANGELOG  

## 下一步

- [Core 概览](./core/overview)  
- [快速开始](./core/quick-start)  
- [路线图](./roadmap)  
