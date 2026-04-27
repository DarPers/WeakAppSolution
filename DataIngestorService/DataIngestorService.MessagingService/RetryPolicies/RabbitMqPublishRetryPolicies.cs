using Microsoft.Extensions.Logging;
using Polly;

namespace DataIngestorService.MessagingService.RetryPolicies;

public static class RabbitMqPublishRetryPolicies
{
    public static IAsyncPolicy GetRabbitMqPublishRetryPolicy(int retryCount, TimeSpan delay, ILogger logger)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(retryCount);

        return Policy
            .Handle<Exception>(ShouldRetry)
            .WaitAndRetryAsync(
                retryCount,
                _ => delay,
                onRetry: (exception, timespan, attemptNumber, _) =>
                {
                    logger.LogWarning(
                        exception,
                        "RabbitMQ publish failed (retry {Attempt}/{MaxRetries}), waiting {DelaySeconds:F1}s",
                        attemptNumber,
                        retryCount,
                        timespan.TotalSeconds);
                });
    }

    private static bool ShouldRetry(Exception ex) =>
        ex is not OperationCanceledException
        && ex is not ObjectDisposedException;
}
