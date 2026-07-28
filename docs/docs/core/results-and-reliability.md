---
title: Result 与可靠性
---

# Result 与可靠性

## CommResult

约定：**链路超时、校验失败、对端拒绝等可预期失败** → `CommResult`；**空引用、错误用法** → 抛异常。

```csharp
CommResult r = CommResult.Failure(CommError.Timeout());
if (r.IsFailure) { /* 记录 r.Error.Code / Message */ }

CommResult<int> value = CommResult<int>.Success(42);
int x = value.Map(v => v + 1).GetValueOrThrow();
```

## RetryPolicy

指数退避 + 可选抖动；默认重试超时、校验失败、瞬时读写错误等。

```csharp
var policy = new RetryPolicy(new RetryOptions
{
    MaxAttempts = 3,
    BaseDelay = TimeSpan.FromMilliseconds(100),
    UseJitter = true,
});

var result = await policy.ExecuteAsync(async ct =>
{
    // 执行一次请求
    return CommResult.Success;
}, cancellationToken);
```

## TimeoutGate

为异步操作附加超时，超时映射为 `CommErrorCode.Timeout`，**不吞掉外部 CancellationToken**。

```csharp
var result = await TimeoutGate.ExecuteAsync(
    async ct => await DoRequestAsync(ct),
    TimeSpan.FromSeconds(3),
    cancellationToken);
```
