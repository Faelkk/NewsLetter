using FluentAssertions;
using Newsletter.Domain.Entities;
using Newsletter.Infrastructure.Repository;
using NewsLetter.Test.Test.Fixtures;
using Xunit;

namespace Newsletter.IntegrationTests.Repositories;

public class SubscriptionRepositoryIntegrationTests : DatabaseTestBase, IClassFixture<PostgresTestContainerFixture>
{
    private readonly string _connectionString;

    public SubscriptionRepositoryIntegrationTests(PostgresTestContainerFixture fixture) : base(fixture)
    {
        _connectionString = fixture.ConnectionString;
    }

    [Fact]
    public async Task Create_And_GetBySubscriptionId()
    {
        var db = new TestDatabaseContext(_connectionString);
        var userRepo = new UserRepository(db);
        var subRepo = new SubscriptionRepository(db);

        var user = await userRepo.CreateAsync(new User
        {
            Id = Guid.NewGuid(),
            Name = "Jane",
            Email = "jane@example.com",
            Password = "123456",
            Role = "User",
            Interests = ["Science"]
        });

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Status = "active",
            Provider = "Stripe",
            Plan = "Premium",
            StartedAt = DateTime.UtcNow
        };

        var created = await subRepo.CreateAsync(subscription);
        var retrieved = await subRepo.GetBySubscriptionIdAsync(subscription.Id);

        created.Should().NotBeNull();
        retrieved.Should().NotBeNull();
        retrieved!.Status.Should().Be("active");
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnSubscription()
    {
        var db = new TestDatabaseContext(_connectionString);
        var userRepo = new UserRepository(db);
        var subRepo = new SubscriptionRepository(db);

        var user = await userRepo.CreateAsync(new User
        {
            Id = Guid.NewGuid(),
            Name = "Mark",
            Email = "mark@test.com",
            Password = "123456",
            Role = "User",
            Interests = ["Gaming"]
        });

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Status = "active",
            Provider = "Stripe"
        };

        await subRepo.CreateAsync(subscription);

        var found = await subRepo.GetByUserIdAsync(user.Id);
        found.Should().NotBeNull();
        found!.Id.Should().Be(subscription.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifySubscription()
    {
        var db = new TestDatabaseContext(_connectionString);
        var userRepo = new UserRepository(db);
        var subRepo = new SubscriptionRepository(db);

        var user = await userRepo.CreateAsync(new User
        {
            Id = Guid.NewGuid(),
            Name = "Lucy",
            Email = "lucy@test.com",
            Password = "123",
            Role = "User",
            Interests = ["Books"]
        });

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Status = "pending",
            Provider = "Stripe"
        };

        await subRepo.CreateAsync(subscription);

        subscription.Status = "active";
        subscription.Plan = "Gold";

        var updated = await subRepo.UpdateAsync(subscription);

        updated.Status.Should().Be("active");
        updated.Plan.Should().Be("Gold");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSubscriptionsByUser()
    {
        var db = new TestDatabaseContext(_connectionString);
        var userRepo = new UserRepository(db);
        var subRepo = new SubscriptionRepository(db);

        var user = await userRepo.CreateAsync(new User
        {
            Id = Guid.NewGuid(),
            Name = "Delete Sub",
            Email = "delete.sub@test.com",
            Password = "123",
            Role = "User",
            Interests = ["X"]
        });

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Status = "active",
            Provider = "Stripe"
        };

        await subRepo.CreateAsync(subscription);

        var deleted = await subRepo.DeleteAsync(user.Id);
        deleted.Should().BeTrue();

        var found = await subRepo.GetByUserIdAsync(user.Id);
        found.Should().BeNull();
    }
}
