using System.ComponentModel.DataAnnotations;

namespace NotificationService.Options;

public class CorsOptions
{
    public const string SectionName = "Cors";

    [Required, MinLength(1)]
    public required string[] AllowedOrigins { get; set; }
}
