using Polly;
using Polly.Extensions.Http;

namespace DataIngestorService.RetryPolicies;

public static class RetryPoliciesExtentions
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                5,
                retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 200))
            );
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (response, timespan) => logger.LogInformation("Circuit broken. Weak API is down"),
                onReset: () => logger.LogInformation("Connection established."),
                onHalfOpen: () => logger.LogInformation("Test request. Checking Weak API...")
            );
    }
}
