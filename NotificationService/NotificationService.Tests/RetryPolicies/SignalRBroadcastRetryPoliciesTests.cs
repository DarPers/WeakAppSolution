using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NotificationService.RetryPolicies;

namespace NotificationService.Tests.RetryPolicies;

public class SignalRBroadcastRetryPoliciesTests
{
    [Fact]
    public async Task GetSignalRBroadcastRetryPolicy_WhenSucceedsFirstAttempt_DoesNotRetry()
    {
        // Arrange
        var policy = SignalRBroadcastRetryPolicies.GetSignalRBroadcastRetryPolicy(
            retryCount: 3,
            delay: TimeSpan.Zero,
            NullLogger.Instance);
        var attemptCount = 0;

        // Act
        await policy.ExecuteAsync(() =>
        {
            attemptCount++;
            return Task.CompletedTask;
        });

        // Assert
        attemptCount.Should().Be(1);
    }

    [Fact]
    public async Task GetSignalRBroadcastRetryPolicy_WhenAlwaysFails_RetriesUntilExhausted()
    {
        // Arrange
        var policy = SignalRBroadcastRetryPolicies.GetSignalRBroadcastRetryPolicy(
            retryCount: 2,
            delay: TimeSpan.Zero,
            NullLogger.Instance);
        var attemptCount = 0;

        // Act
        var act = async () => await policy.ExecuteAsync(() =>
        {
            attemptCount++;
            throw new InvalidOperationException("fail");
        });

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        attemptCount.Should().Be(3);
    }

    [Fact]
    public async Task GetSignalRBroadcastRetryPolicy_WhenOperationCanceled_DoesNotRetry()
    {
        // Arrange
        var policy = SignalRBroadcastRetryPolicies.GetSignalRBroadcastRetryPolicy(
            retryCount: 3,
            delay: TimeSpan.Zero,
            NullLogger.Instance);
        var attemptCount = 0;

        // Act
        var act = async () => await policy.ExecuteAsync(() =>
        {
            attemptCount++;
            throw new OperationCanceledException();
        });

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
        attemptCount.Should().Be(1);
    }
}
