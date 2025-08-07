using FluentAssertions;
using Newsletter.Domain.Entities;
using Newsletter.Infrastructure.Repository;
using NewsLetter.Test.Test.Fixtures;
using Xunit;

namespace NewsLetter.Test.Test.Repositories;

public class UserRepositoryIntegrationTests : DatabaseTestBase, IClassFixture<PostgresTestContainerFixture>
{
    private readonly string _connectionString;

    public UserRepositoryIntegrationTests(PostgresTestContainerFixture fixture) : base(fixture)
    {
        _connectionString = fixture.ConnectionString;
    }

    [Fact]
    public async Task Create_And_GetById()
    {
        var db = new TestDatabaseContext(_connectionString);
        var repo = new UserRepository(db);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "User Test",
            Email = "user@test.com",
            Password = "123456",
            Role = "Admin",
            Interests = ["Tech", "News"]
        };

        var created = await repo.CreateAsync(user);
        var retrieved = await repo.GetByIdAsync(user.Id);

        retrieved.Should().NotBeNull();
        retrieved!.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetByIdEmail_ShouldReturnUser()
    {
        var db = new TestDatabaseContext(_connectionString);
        var repo = new UserRepository(db);

        var user = await repo.CreateAsync(new User
        {
            Id = Guid.NewGuid(),
            Name = "Email Test",
            Email = "email@test.com",
            Password = "123",
            Role = "User",
            Interests = ["A"]
        });

        var found = await repo.GetByIdEmail("email@test.com");

        found.Should().NotBeNull();
        found!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldChangeUser()
    {
        var db = new TestDatabaseContext(_connectionString);
        var repo = new UserRepository(db);

        var user = await repo.CreateAsync(new User
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Email = "old@test.com",
            Password = "123",
            Role = "User",
            Interests = ["X"]
        });

        user = new User
        {
            Id = user.Id,
            Name = "New Name",
            Email = "new@test.com",
            Password = "321",
            Role = "Admin",
            Interests = ["Y"]
        };

        var updated = await repo.UpdateAsync(user);

        updated.Name.Should().Be("New Name");
        updated.Email.Should().Be("new@test.com");
        updated.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveUser()
    {
        var db = new TestDatabaseContext(_connectionString);
        var repo = new UserRepository(db);

        var user = await repo.CreateAsync(new User
        {
            Id = Guid.NewGuid(),
            Name = "To Delete",
            Email = "delete@test.com",
            Password = "123",
            Role = "User",
            Interests = ["Z"]
        });

        var deleted = await repo.DeleteAsync(user.Id);
        deleted.Should().BeTrue();

        var found = await repo.GetByIdAsync(user.Id);
        found.Should().BeNull();
    }
}
