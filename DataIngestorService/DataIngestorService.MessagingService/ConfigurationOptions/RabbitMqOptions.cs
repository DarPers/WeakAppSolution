using System.ComponentModel.DataAnnotations;

namespace DataIngestorService.MessagingService.ConfigurationOptions;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    [Required, MinLength(1)]
    public required string HostName { get; set; }

    [Required, MinLength(1)]
    public required string VirtualHostName { get; set; }

    [Required, MinLength(1)]
    public required string UserName { get; set; }

    [Required, MinLength(1)]
    public required string Password { get; set; }

    [Range(0, 30)]
    public int PublishRetryCount { get; set; } = 3;

    [Range(0, 300_000)]
    public int PublishRetryDelayMilliseconds { get; set; } = 1000;
}
