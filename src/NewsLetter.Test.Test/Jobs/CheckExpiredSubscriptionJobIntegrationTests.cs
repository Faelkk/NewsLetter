using Dapper;
using FluentAssertions;
using Newsletter.Domain.Entities;
using Newsletter.Infrastructure.Context;
using Newsletter.Infrastructure.Repository;
using NewsLetter.Test.Test.Fixtures;
using Npgsql;

namespace NewsLetter.Test.Test.Jobs;

public class CheckExpiredSubscriptionJobIntegrationTests : IClassFixture<PostgresTestContainerFixture>, IAsyncLifetime
{
    private readonly PostgresTestContainerFixture _fixture;
    private DatabaseContext _dbContext = null!;
    private SubscriptionRepository _subscriptionRepo = null!;
    private CheckExpiredSubscriptionJob _job = null!;

    public CheckExpiredSubscriptionJobIntegrationTests(PostgresTestContainerFixture fixture)
    {
        _fixture = fixture;
    }

   public async Task InitializeAsync()
{
    await _fixture.ResetDatabase();

    _dbContext = new DatabaseContext(_fixture.Configuration);
    _subscriptionRepo = new SubscriptionRepository(_dbContext);
    _job = new CheckExpiredSubscriptionJob(_subscriptionRepo);

    var userId = Guid.NewGuid();

    // Insere o usuário no banco
    await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
    await conn.OpenAsync();

    var insertUserSql = @"
        INSERT INTO Users (id, name, email, password, role, interests)
        VALUES (@Id, @Name, @Email, @Password, @Role, @Interests)";

    await conn.ExecuteAsync(insertUserSql, new {
        Id = userId,
        Name = "Test User",
        Email = "test@example.com",
        Password = "hashedpassword",
        Role = "User",
        Interests = new string[] { "tech", "news" }
    });
    
    await _subscriptionRepo.CreateAsync(new Subscription
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        Status = "active",
        ExpiresAt = DateTime.UtcNow.AddDays(-1), 
        UpdatedAt = DateTime.UtcNow
    });
}

    [Fact]
    public async Task Execute_ShouldSetExpiredSubscriptionsToPending()
    {
        await _job.Execute();

        var subs = await _subscriptionRepo.GetAllAsync();

        foreach (var sub in subs)
        {
            if (sub.ExpiresAt <= DateTime.UtcNow.Date)
            {
                sub.Status.Should().Be("pending");
            }
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;
}