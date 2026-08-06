using System.ComponentModel.DataAnnotations;

namespace NotificationService.Options;

public class RabbitMqOptions
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

    [Required, MinLength(1)]
    public required string QueueName { get; set; }
}
