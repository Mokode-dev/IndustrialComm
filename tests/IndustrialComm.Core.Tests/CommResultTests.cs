using IndustrialComm.Results;
using Xunit;

namespace IndustrialComm.Core.Tests;

public class CommResultTests
{
    [Fact]
    public void Success_And_Failure()
    {
        Assert.True(CommResult.Success.IsSuccess);
        Assert.Null(CommResult.Success.Error);

        var fail = CommResult.Failure(CommError.Timeout());
        Assert.True(fail.IsFailure);
        Assert.Equal(CommErrorCode.Timeout, fail.Error!.Code);
    }

    [Fact]
    public void Generic_Map_And_GetValueOrThrow()
    {
        var ok = CommResult<int>.Success(40).Map(x => x + 2);
        Assert.Equal(42, ok.GetValueOrThrow());

        var fail = CommResult<int>.Failure(CommError.Disconnected());
        Assert.Throws<InvalidOperationException>(() => fail.GetValueOrThrow());
        Assert.True(fail.Map(x => x + 1).IsFailure);
    }

    [Fact]
    public void FromException_MapsCancellation()
    {
        var error = CommError.FromException(new OperationCanceledException("cancelled"));
        Assert.Equal(CommErrorCode.Cancelled, error.Code);
    }
}
