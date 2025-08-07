using FluentAssertions;
using Newsletter.Domain.Entities;
using Newsletter.Infrastructure.Repository;
using NewsLetter.Test.Test.Fixtures;
using Xunit;

namespace Newsletter.IntegrationTests.Repositories;

public class NewsletterRepositoryIntegrationTests : DatabaseTestBase, IClassFixture<PostgresTestContainerFixture>
{
    private readonly string _connectionString;

    public NewsletterRepositoryIntegrationTests(PostgresTestContainerFixture fixture) : base(fixture)
    {
        _connectionString = fixture.ConnectionString;
    }

    [Fact]
    public async Task GenerateAndSendAsync_And_GetByIdAsync()
    {
        var db = new TestDatabaseContext(_connectionString);
        var userRepo = new UserRepository(db);
        var newsRepo = new NewsletterRepository(db);

        var user = await userRepo.CreateAsync(new User
        {
            Id = Guid.NewGuid(),
            Name = "Alice",
            Email = "alice@example.com",
            Password = "123456",
            Role = "User",
            Interests = ["Books", "Travel" ],
        });

        var newsletter = new NewsletterEntry
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Topics = ["Books", "Travel" ],
            Content = "Sample newsletter content",
            Sent = false,
            CreatedAt = DateTime.UtcNow
        };

        var created = await newsRepo.GenerateAndSendAsync(newsletter);
        var retrieved = await newsRepo.GetByIdAsync(user.Id, newsletter.Id);

        created.Should().NotBeNull();
        retrieved.Should().NotBeNull();
        retrieved!.Content.Should().Be(newsletter.Content);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnNewsletters()
    {
        var db = new TestDatabaseContext(_connectionString);
        var userRepo = new UserRepository(db);
        var newsRepo = new NewsletterRepository(db);

        var user = await userRepo.CreateAsync(new User
        {
            Id = Guid.NewGuid(),
            Name = "Bob",
            Email = "bob@example.com",
            Password = "123",
            Role = "User",
            Interests = ["Movies"],
        });

        var newsletter = new NewsletterEntry
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Topics = ["Movies"],
            Content = "Movie news",
            Sent = false,
            CreatedAt = DateTime.UtcNow
        };

        await newsRepo.GenerateAndSendAsync(newsletter);

        var list = await newsRepo.GetByUserIdAsync(user.Id);

        list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DeleteAsyncById_ShouldDeleteNewsletter()
    {
        var db = new TestDatabaseContext(_connectionString);
        var userRepo = new UserRepository(db);
        var newsRepo = new NewsletterRepository(db);

        var user = await userRepo.CreateAsync(new User
        {
            Id = Guid.NewGuid(),
            Name = "Carl",
            Email = "carl@example.com",
            Password = "123",
            Role = "User",
            Interests = ["Sports"]
        });

        var newsletter = new NewsletterEntry
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Topics = ["Sports"],
            Content = "Sports news",
            Sent = false,
            CreatedAt = DateTime.UtcNow
        };

        await newsRepo.GenerateAndSendAsync(newsletter);

        var deleted = await newsRepo.DeleteAsyncById(user.Id, newsletter.Id);
        deleted.Should().BeTrue();

        var found = await newsRepo.GetByIdAsync(user.Id, newsletter.Id);
        found.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteAllUserNewsletters()
    {
        var db = new TestDatabaseContext(_connectionString);
        var userRepo = new UserRepository(db);
        var newsRepo = new NewsletterRepository(db);

        var user = await userRepo.CreateAsync(new User
        {
            Id = Guid.NewGuid(),
            Name = "Dave",
            Email = "dave@example.com",
            Password = "123",
            Role = "User",
            Interests = ["Tech"],
        });

        var newsletter1 = new NewsletterEntry
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Topics = ["Tech"],
            Content = "Tech news 1",
            Sent = false,
            CreatedAt = DateTime.UtcNow
        };

        var newsletter2 = new NewsletterEntry
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Topics = ["Tech"],
            Content = "Tech news 2",
            Sent = false,
            CreatedAt = DateTime.UtcNow
        };

        await newsRepo.GenerateAndSendAsync(newsletter1);
        await newsRepo.GenerateAndSendAsync(newsletter2);

        var deleted = await newsRepo.DeleteAsync(user.Id);
        deleted.Should().BeTrue();

        var newsletters = await newsRepo.GetByUserIdAsync(user.Id);
        newsletters.Should().BeEmpty();
    }
}
