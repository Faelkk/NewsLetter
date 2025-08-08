using Dapper;
using Moq;
using FluentAssertions;
using Newsletter.Application.Jobs;
using Newsletter.Domain.Entities;
using Newsletter.Infrastructure.Context;
using Newsletter.Infrastructure.Repository;
using Newsletter.Domain.Interfaces;
using Newsletter.Application.Interfaces;
using NewsLetter.Test.Test.Fixtures;
using Npgsql;
using Xunit.Abstractions;

namespace Newsletter.Tests.Integration.Jobs;

public class SendMonthlyEmailsJobIntegrationTests : IClassFixture<PostgresTestContainerFixture>, IAsyncLifetime
{
    private readonly PostgresTestContainerFixture _fixture;
    private DatabaseContext _dbContext = null!;
    private SubscriptionRepository _subscriptionRepo = null!;
    private SendMonthlyEmailsJob _job = null!;
    private Mock<IEmailService> _emailServiceMock = null!;
    private Mock<IGenerateNewsLetters> _generateNewsLettersMock = null!;
    private readonly ITestOutputHelper _output;

    public SendMonthlyEmailsJobIntegrationTests(PostgresTestContainerFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }


    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabase();

        _dbContext = new DatabaseContext(_fixture.Configuration);
        _subscriptionRepo = new SubscriptionRepository(_dbContext);

        _emailServiceMock = new Mock<IEmailService>();
        _generateNewsLettersMock = new Mock<IGenerateNewsLetters>();


        var userId = Guid.NewGuid();
        
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
        
      var today = DateTime.UtcNow.Date;
await _subscriptionRepo.CreateAsync(new Subscription
{
    Id = Guid.NewGuid(),
    UserId = userId,
    Status = "Active",
    NextDeliveryDate = today,
    UpdatedAt = DateTime.UtcNow,
});


        _generateNewsLettersMock
            .Setup(g => g.GenerateNewsLetterAndSave(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync("Teste body");
        
        var subs = await _subscriptionRepo.GetAllAsync();
        Assert.NotEmpty(subs);

    
        _job = new SendMonthlyEmailsJob(_subscriptionRepo, _emailServiceMock.Object, _generateNewsLettersMock.Object);
    }


    [Fact]
    public async Task Execute_ShouldSendEmailsAndUpdateNextDeliveryDate()
    {
        var beforeUpdate = DateTime.UtcNow;

        await _job.Execute();

        _generateNewsLettersMock.Verify(g => g.GenerateNewsLetterAndSave(It.IsAny<Guid>(), It.IsAny<string>()),
            Times.AtLeastOnce);
        _emailServiceMock.Verify(e => e.SendMonthlyEmail(It.IsAny<string>(), "Teste body"));

        var subscriptions = await _subscriptionRepo.GetAllAsync();
        foreach (var sub in subscriptions)
        {
            

            sub.UpdatedAt.Should().BeAfter(beforeUpdate);
        }
    }


    public Task DisposeAsync() => Task.CompletedTask;
}
