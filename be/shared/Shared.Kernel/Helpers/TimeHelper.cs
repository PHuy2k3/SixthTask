namespace Shared.Kernel;

public static class TimeHelper
{
    public static string ToTimeAgo(this DateTime time)
    {
        var span = DateTime.UtcNow - time;

        if (span.TotalSeconds < 60)
            return $"{span.Seconds}s ago";

        if (span.TotalMinutes < 60)
            return $"{span.Minutes}m ago";

        if (span.TotalHours < 24)
            return $"{span.Hours}h ago";

        return $"{span.Days}d ago";
    }
}
