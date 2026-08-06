using Microsoft.Extensions.Logging;
using Polly;

namespace NotificationService.RetryPolicies;

public static class SignalRBroadcastRetryPolicies
{
    public static IAsyncPolicy GetSignalRBroadcastRetryPolicy(int retryCount, TimeSpan delay, ILogger logger)
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
                        "SignalR broadcast failed (retry {Attempt}/{MaxRetries}), waiting {DelaySeconds:F1}s",
                        attemptNumber,
                        retryCount,
                        timespan.TotalSeconds);
                });
    }

    private static bool ShouldRetry(Exception ex) =>
        ex is not OperationCanceledException
        && ex is not ObjectDisposedException;
}
