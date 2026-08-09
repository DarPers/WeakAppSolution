using System.ComponentModel.DataAnnotations;

namespace DataProcessorService.DAL.ConfigurationOptions;

public sealed class PostgresOptions
{
    public const string SectionName = "ConnectionStrings";

    [Required, MinLength(1)]
    public required string PostgresSQLConnectionString { get; set; }
}
