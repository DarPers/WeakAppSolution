using System.Net;
using DataIngestorService.RetryPolicies;
using FluentAssertions;

namespace DataIngestorService.Tests.RetryPolicies;

public class RetryPolicyTests
{
    private static readonly Func<int, TimeSpan> NoDelay = _ => TimeSpan.Zero;

    [Fact]
    public async Task GetRetryPolicy_WhenServerReturns200_ReturnsSuccessOnFirstAttempt()
    {
        // Arrange
        var policy = RetryPoliciesExtentions.GetRetryPolicy(NoDelay);
        var attemptCount = 0;

        // Act
        var response = await policy.ExecuteAsync(() =>
        {
            attemptCount++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });

        // Assert
        attemptCount.Should().Be(1);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetRetryPolicy_WhenServerReturns500_RetriesUntilAllAttemptsExhausted()
    {
        // Arrange
        var policy = RetryPoliciesExtentions.GetRetryPolicy(NoDelay);
        var attemptCount = 0;

        // Act
        var response = await policy.ExecuteAsync(() =>
        {
            attemptCount++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        });

        // Assert
        attemptCount.Should().Be(6);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetRetryPolicy_WhenServerReturns429_DoesNotRetry()
    {
        // Arrange
        var policy = RetryPoliciesExtentions.GetRetryPolicy(NoDelay);
        var attemptCount = 0;

        // Act
        var response = await policy.ExecuteAsync(() =>
        {
            attemptCount++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.TooManyRequests));
        });

        // Assert
        attemptCount.Should().Be(1);
        response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task GetRetryPolicy_WhenServerReturns500Then200_ReturnsSuccessAfterRetries()
    {
        // Arrange
        var policy = RetryPoliciesExtentions.GetRetryPolicy(NoDelay);
        var attemptCount = 0;

        // Act
        var response = await policy.ExecuteAsync(() =>
        {
            attemptCount++;
            var statusCode = attemptCount < 3
                ? HttpStatusCode.InternalServerError
                : HttpStatusCode.OK;
            return Task.FromResult(new HttpResponseMessage(statusCode));
        });

        // Assert
        attemptCount.Should().Be(3);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
