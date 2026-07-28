using System.IO;
using IndustrialComm.Diagnostics;
using IndustrialComm.Reliability;
using IndustrialComm.Results;
using IndustrialComm.Transport;
using Xunit;

namespace IndustrialComm.Core.Tests;

public class TransportAndReliabilityTests
{
    [Fact]
    public async Task StreamByteTransport_RoundTrip()
    {
        await using var stream = new MemoryStream();
        var diagnostics = new CountingCommDiagnostics();
        await using var transport = new StreamByteTransport(stream, leaveOpen: true, diagnostics: diagnostics);

        await transport.ConnectAsync();
        var payload = new byte[] { 0x10, 0x20, 0x30 };
        await transport.WriteAsync(payload);

        stream.Position = 0;
        var readBuffer = new byte[8];
        var n = await transport.ReadAsync(readBuffer);
        Assert.Equal(3, n);
        Assert.Equal(payload, readBuffer.AsSpan(0, 3).ToArray());
        Assert.Equal(3, diagnostics.BytesSentTotal);
        Assert.Equal(3, diagnostics.BytesReceivedTotal);
    }

    [Fact]
    public async Task RetryPolicy_RetriesThenSucceeds()
    {
        var attempts = 0;
        var policy = new RetryPolicy(new RetryOptions
        {
            MaxAttempts = 3,
            BaseDelay = TimeSpan.FromMilliseconds(1),
            UseJitter = false,
        });

        var result = await policy.ExecuteAsync(async ct =>
        {
            attempts++;
            await Task.Yield();
            if (attempts < 3)
            {
                return CommResult.Failure(CommError.Timeout());
            }

            return CommResult.Success;
        });

        Assert.True(result.IsSuccess);
        Assert.Equal(3, attempts);
    }

    [Fact]
    public async Task RetryPolicy_DoesNotRetryNonRetryable()
    {
        var attempts = 0;
        var policy = new RetryPolicy(new RetryOptions
        {
            MaxAttempts = 5,
            BaseDelay = TimeSpan.FromMilliseconds(1),
        });

        var result = await policy.ExecuteAsync(ct =>
        {
            attempts++;
            return new ValueTask<CommResult>(
                CommResult.Failure(CommError.InvalidArgument("bad")));
        });

        Assert.True(result.IsFailure);
        Assert.Equal(1, attempts);
        Assert.Equal(CommErrorCode.InvalidArgument, result.Error!.Code);
    }

    [Fact]
    public async Task TimeoutGate_TimesOut()
    {
        var result = await TimeoutGate.ExecuteAsync(
            async ct =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5), ct);
                return CommResult.Success;
            },
            TimeSpan.FromMilliseconds(50));

        Assert.True(result.IsFailure);
        Assert.Equal(CommErrorCode.Timeout, result.Error!.Code);
    }

    [Fact]
    public void CommOptions_Validate()
    {
        var options = new IndustrialComm.Options.CommOptions
        {
            Timeout = TimeSpan.FromSeconds(1),
            MaxFrameLength = 1024,
            ReceiveBufferSize = 2048,
        };
        options.Validate();

        options.ReceiveBufferSize = 512;
        Assert.Throws<ArgumentOutOfRangeException>(() => options.Validate());
    }
}
