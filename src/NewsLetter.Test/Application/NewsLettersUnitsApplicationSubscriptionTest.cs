using Moq;
using Newsletter.Application.DTOS.Subscriptions;
using Newsletter.Application.Interfaces;
using Newsletter.Application.Services;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;
using Newsletter.Infrastructure.Interfaces;
using Newsletter.Presentation.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Newsletter.Test.Application;

public class SubscriptionServiceTests
{
    private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IStripeSubscriptionService> _stripeSubscriptionServiceMock = new();
    private readonly SubscriptionService _service;

    public SubscriptionServiceTests()
    {
        _service = new SubscriptionService(
            _subscriptionRepositoryMock.Object,
            _userRepositoryMock.Object,
            _stripeSubscriptionServiceMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllSubscriptions()
    {
        var list = new List<Subscription>
        {
            new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Provider = "Stripe",
                Status = "active",
                StartedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMonths(1),
                UpdatedAt = DateTime.UtcNow
            }
        };

        _subscriptionRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

        var result = await _service.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("Stripe", result.First().Provider);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsSubscription_WhenExists()
    {
        var userId = Guid.NewGuid();
        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Provider = "Stripe",
            Status = "active",
            StartedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMonths(1),
            UpdatedAt = DateTime.UtcNow
        };

        _subscriptionRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(subscription);

        var result = await _service.GetByUserIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result!.UserId);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsNull_WhenNotFound()
    {
        _subscriptionRepositoryMock.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync((Subscription?)null);

        var result = await _service.GetByUserIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenUserNotFound()
    {
        var request = new CreateSubscriptionRequest(Guid.NewGuid(), "Stripe", "IdPlan");

        _userRepositoryMock.Setup(r => r.GetByIdAsync(request.UserId)).ReturnsAsync((Domain.Entities.User?)null);

        await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenSubscriptionExists()
    {
        var userId = Guid.NewGuid();
        var request = new CreateSubscriptionRequest(userId, "Stripe", "IdPlan");

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new Domain.Entities.User
        {
            Id = userId,
            Name = "User",
            Email = "user@example.com",
            Password = "hashed",
            Role = "User",
            Interests = [],
        });

        _subscriptionRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(new Subscription());

        await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_ReturnsSubscriptionDto_WhenSuccess()
    {
        var userId = Guid.NewGuid();
        var request = new CreateSubscriptionRequest(userId, "Stripe", "IdPlan");

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new Domain.Entities.User
        {
            Id = userId,
            Name = "User",
            Email = "user@example.com",
            Password = "hashed",
            Role = "User",
            Interests = [],
        });

        _subscriptionRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((Subscription?)null);

        _subscriptionRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Subscription>()))
            .ReturnsAsync((Subscription s) => s);

        var result = await _service.CreateAsync(request);

        Assert.Equal(userId, result.UserId);
        Assert.Equal("pending", result.Status);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenSubscriptionNotFound()
    {
        var id = new Guid();
        var externalId = "IdTeste";
        var request = new UpdateSubscriptionRequest(externalId, "Stripe",
            "pending",
            "IdPlan",
            DateTime.UtcNow.AddMonths(1),
          DateTime.UtcNow,DateTime.UtcNow.AddMonths(1),DateTime.Now);

        _subscriptionRepositoryMock.Setup(r => r.GetByUserIdAsync(id)).ReturnsAsync((Subscription?)null);

        var result = await _service.UpdateAsync(request, id);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsUpdatedSubscriptionDto_WhenSuccess()
    {
        var id = Guid.NewGuid();
        var original = new Subscription
        {
            Id = id,
            UserId = id,
            Provider = "Stripe",
            Status = "pending",
            StartedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMonths(1),
            UpdatedAt = DateTime.UtcNow
        };
        var request = new UpdateSubscriptionRequest
        (
           "ExternalSubscriptionId",
      "Stripe","Active", "IdPlan",new DateTime(),new DateTime(),new DateTime(),new DateTime());
        
        _subscriptionRepositoryMock.Setup(r => r.GetByUserIdAsync(id)).ReturnsAsync(original);

        _subscriptionRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Subscription>()))
            .ReturnsAsync((Subscription s) => s);

        var result = await _service.UpdateAsync(request, id);

        Assert.NotNull(result);
        Assert.Equal("Active", result!.Status);
    }

    [Fact]
    public async Task DeleteByUserAsync_ReturnsFalse_WhenSubscriptionNotFound()
    {
        var userId = Guid.NewGuid();

        _subscriptionRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((Subscription?)null);

        var result = await _service.DeleteByUserAsync(userId);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteByUserAsync_CancelsStripeSubscription_AndReturnsTrue()
    {
        var userId = Guid.NewGuid();
        var subscription = new Subscription
        {
            UserId = userId,
            ExternalSubscriptionId = "stripe_123",
            Provider = "Stripe"
        };

        _subscriptionRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(subscription);
        _stripeSubscriptionServiceMock.Setup(s => s.CancelSubscriptionAsync("stripe_123")).Returns(Task.CompletedTask);
        _subscriptionRepositoryMock.Setup(r => r.DeleteAsync(userId)).ReturnsAsync(true);

        var result = await _service.DeleteByUserAsync(userId);

        _stripeSubscriptionServiceMock.Verify(s => s.CancelSubscriptionAsync("stripe_123"), Times.Once);
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteByUserAsync_ThrowsException_WhenStripeCancelFails()
    {
        var userId = Guid.NewGuid();
        var subscription = new Subscription
        {
            UserId = userId,
            ExternalSubscriptionId = "stripe_123",
            Provider = "Stripe"
        };

        _subscriptionRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(subscription);
        _stripeSubscriptionServiceMock.Setup(s => s.CancelSubscriptionAsync("stripe_123"))
            .ThrowsAsync(new Exception("Stripe error"));

        await Assert.ThrowsAsync<Exception>(() => _service.DeleteByUserAsync(userId));
    }
}
