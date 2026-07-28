---
title: 错误码一览
---

# CommErrorCode

| 码 | 名称 | 含义 |
|----|------|------|
| 0 | Unknown | 未分类 |
| 1 | Timeout | 超时 |
| 2 | Disconnected | 未连接 |
| 3 | ConnectFailed | 连接失败 |
| 4 | DisconnectFailed | 断开失败 |
| 5 | ReadFailed | 读失败 |
| 6 | WriteFailed | 写失败 |
| 7 | ChecksumMismatch | 校验失败 |
| 8 | FramingError | 帧错误 |
| 9 | BufferOverflow | 缓冲溢出 |
| 10 | ProtocolError | 协议否定应答 |
| 11 | InvalidAddress | 非法地址/数量 |
| 12 | InvalidArgument | 非法参数 |
| 13 | Cancelled | 已取消 |
| 14 | RetryExhausted | 重试耗尽 |
| 15 | ConnectionClosed | 对端关闭 |
| 16 | NotSupported | 不支持 |
| 17 | BadQuality | 数据质量坏 |

协议包可将厂商错误映射到上述稳定码，并在 `CommError.Detail` 中保留原始信息。
