using System.ComponentModel.DataAnnotations;

namespace DataIngestorService.ConfigurationOptions;

public sealed class WeakAppOptions
{
    public const string SectionName = "WeakAppSettings";

    [Required, Url]
    public required string BaseUrl { get; init; }

    [Required, MinLength(1)]
    public required string AuthorizationKeyValue { get; init; }
}
