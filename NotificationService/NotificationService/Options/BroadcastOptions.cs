using System.ComponentModel.DataAnnotations;

namespace NotificationService.Options;

public sealed class BroadcastOptions
{
    public const string SectionName = "Broadcast";

    [Range(0, 30)]
    public int RetryCount { get; set; } = 3;

    [Range(0, 300_000)]
    public int RetryDelayMilliseconds { get; set; } = 1000;
}
