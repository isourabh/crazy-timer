using System.Globalization;

namespace CrazyTimer;

public sealed class StopwatchModel(TimeProvider? timeProvider = null)
{
    private readonly TimeProvider clock = timeProvider ?? TimeProvider.System;
    private TimeSpan accumulated;
    private long startedAt;

    public bool IsRunning { get; private set; }

    public TimeSpan Elapsed => accumulated +
        (IsRunning ? clock.GetElapsedTime(startedAt) : TimeSpan.Zero);

    public void Start()
    {
        if (IsRunning)
            return;

        startedAt = clock.GetTimestamp();
        IsRunning = true;
    }

    public void Stop()
    {
        if (!IsRunning)
            return;

        accumulated = Elapsed;
        IsRunning = false;
    }

    public void Reset()
    {
        IsRunning = false;
        accumulated = TimeSpan.Zero;
    }

    public static string FormatElapsed(TimeSpan elapsed) =>
        string.Create(CultureInfo.InvariantCulture,
            $"{elapsed.Ticks / TimeSpan.TicksPerHour:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}.{elapsed.Milliseconds / 10:00}");
}
