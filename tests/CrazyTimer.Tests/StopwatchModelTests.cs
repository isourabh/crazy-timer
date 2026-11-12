namespace CrazyTimer.Tests;

public sealed class StopwatchModelTests
{
    [Fact]
    public void StartStopAndResumeAccumulateOnlyRunningTime()
    {
        var clock = new ManualClock();
        var stopwatch = new StopwatchModel(clock);
        Assert.False(stopwatch.IsRunning);
        Assert.Equal(TimeSpan.Zero, stopwatch.Elapsed);

        stopwatch.Start();
        clock.Advance(TimeSpan.FromSeconds(2));
        stopwatch.Start();
        clock.Advance(TimeSpan.FromSeconds(3));
        Assert.True(stopwatch.IsRunning);
        Assert.Equal(TimeSpan.FromSeconds(5), stopwatch.Elapsed);

        stopwatch.Stop();
        clock.Advance(TimeSpan.FromHours(1));
        stopwatch.Stop();
        Assert.False(stopwatch.IsRunning);
        Assert.Equal(TimeSpan.FromSeconds(5), stopwatch.Elapsed);

        stopwatch.Start();
        clock.Advance(TimeSpan.FromMilliseconds(120));
        stopwatch.Stop();
        Assert.Equal(TimeSpan.FromMilliseconds(5120), stopwatch.Elapsed);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ResetStopsAndClearsThenAllowsAFreshStart(bool resetWhileRunning)
    {
        var clock = new ManualClock();
        var stopwatch = new StopwatchModel(clock);
        stopwatch.Start();
        clock.Advance(TimeSpan.FromMinutes(2));
        if (!resetWhileRunning)
            stopwatch.Stop();

        stopwatch.Reset();
        clock.Advance(TimeSpan.FromMinutes(3));
        Assert.False(stopwatch.IsRunning);
        Assert.Equal(TimeSpan.Zero, stopwatch.Elapsed);

        stopwatch.Start();
        clock.Advance(TimeSpan.FromSeconds(1));
        Assert.Equal(TimeSpan.FromSeconds(1), stopwatch.Elapsed);
    }

    [Theory]
    [InlineData(0, "00:00:00.00")]
    [InlineData(9, "00:00:00.00")]
    [InlineData(10, "00:00:00.01")]
    [InlineData(59999, "00:00:59.99")]
    [InlineData(60000, "00:01:00.00")]
    [InlineData(3599999, "00:59:59.99")]
    [InlineData(3600000, "01:00:00.00")]
    [InlineData(86400000, "24:00:00.00")]
    [InlineData(360000000, "100:00:00.00")]
    public void FormatsHundredthsAndUnboundedHours(long milliseconds, string expected)
    {
        Assert.Equal(expected, StopwatchModel.FormatElapsed(TimeSpan.FromMilliseconds(milliseconds)));
    }

    private sealed class ManualClock : TimeProvider
    {
        private long timestamp;
        public override long TimestampFrequency => TimeSpan.TicksPerSecond;
        public override long GetTimestamp() => timestamp;
        public void Advance(TimeSpan duration) => timestamp += duration.Ticks;
    }
}
