using System.ComponentModel.DataAnnotations;
using DataProcessorService.ConfigurationOptions;
using DataProcessorService.DAL.ConfigurationOptions;
using FluentAssertions;

namespace DataProcessorService.Tests.ConfigurationOptions;

public class OptionsValidationTests
{
    [Fact]
    public void RabbitMqOptions_WhenValid_PassesValidation()
    {
        var options = new RabbitMqOptions
        {
            HostName = "rabbitmq",
            VirtualHostName = "/",
            UserName = "guest",
            Password = "guest",
            QueueName = "processor-sensor-events",
            ConsumeRetryCount = 3,
            ConsumeRetryDelayMilliseconds = 1000
        };

        Validate(options).Should().BeEmpty();
    }

    [Fact]
    public void RabbitMqOptions_WhenPasswordMissing_FailsValidation()
    {
        var options = new RabbitMqOptions
        {
            HostName = "rabbitmq",
            VirtualHostName = "/",
            UserName = "guest",
            Password = "",
            QueueName = "processor-sensor-events"
        };

        Validate(options).Should().Contain(r => r.MemberNames.Contains(nameof(RabbitMqOptions.Password)));
    }

    [Fact]
    public void RabbitMqOptions_WhenRetryCountOutOfRange_FailsValidation()
    {
        var options = new RabbitMqOptions
        {
            HostName = "rabbitmq",
            VirtualHostName = "/",
            UserName = "guest",
            Password = "guest",
            QueueName = "processor-sensor-events",
            ConsumeRetryCount = 31
        };

        Validate(options).Should().Contain(r => r.MemberNames.Contains(nameof(RabbitMqOptions.ConsumeRetryCount)));
    }

    [Fact]
    public void PostgresOptions_WhenValid_PassesValidation()
    {
        var options = new PostgresOptions
        {
            PostgresSQLConnectionString = "Host=postgres;Database=db;Username=postgres"
        };

        Validate(options).Should().BeEmpty();
    }

    [Fact]
    public void PostgresOptions_WhenConnectionStringMissing_FailsValidation()
    {
        var options = new PostgresOptions
        {
            PostgresSQLConnectionString = ""
        };

        Validate(options)
            .Should()
            .Contain(r => r.MemberNames.Contains(nameof(PostgresOptions.PostgresSQLConnectionString)));
    }

    private static List<ValidationResult> Validate(object options)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(options, new ValidationContext(options), results, validateAllProperties: true);
        return results;
    }
}
