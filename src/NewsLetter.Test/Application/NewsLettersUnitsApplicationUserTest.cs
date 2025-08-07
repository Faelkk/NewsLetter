using Moq;
using Newsletter.Application.DTOS.Users;
using Newsletter.Application.Interfaces;
using Newsletter.Application.Services;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;
using Xunit;

namespace Newsletter.Test.Application;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_userRepositoryMock.Object, _passwordHasherMock.Object, _jwtServiceMock.Object);
    }

    [Fact]
    public async Task GetAsync_ReturnsAllUsers()
    {
        var users = new List<User>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "User1",
                Email = "user1@example.com",
                Password = "hashed",
                Interests = new[] { "Tech" },
                Role = "User"
            }
        };

        _userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var result = await _service.GetAsync();

        Assert.Single(result);
        Assert.Equal("User1", result.First().Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenExists()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Name = "Test",
            Email = "test@example.com",
            Password = "hashed",
            Interests = new[] { "AI" },
            Role = "User"
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var result = await _service.GetByIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal("Test", result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var userId = Guid.NewGuid();

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        var result = await _service.GetByIdAsync(userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenEmailExists()
    {
        var request = new CreateUserRequest("Name", "email@test.com", "123", ["Tecnologia"]);

        _userRepositoryMock.Setup(r => r.GetByIdEmail(request.Email))
            .ReturnsAsync(new User
            {
                Id = Guid.NewGuid(),
                Name = "Rafael",
                Email = "rafael@example.com",
                Password = "hashed123",
                Role = "User",
                Interests = Array.Empty<string>()
            });


        await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_ReturnsToken_WhenSuccess()
    {
        var request = new CreateUserRequest("Name", "email@test.com", "123" ,["Tecnologia"]);

        _userRepositoryMock.Setup(r => r.GetByIdEmail(request.Email)).ReturnsAsync((User?)null);
        _passwordHasherMock.Setup(p => p.HashPassword(It.IsAny<User>(), request.Password)).Returns("hashed123");
        _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);
        _jwtServiceMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("token123");

        var result = await _service.CreateAsync(request);

        Assert.Equal("token123", result.Token);
    }

    [Fact]
    public async Task LoginAsync_Throws_WhenUserNotFound()
    {
        var request = new LoginUserRequest("email@test.com", "123");

        _userRepositoryMock.Setup(r => r.GetByIdEmail(request.Email)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<Exception>(() => _service.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_Throws_WhenPasswordInvalid()
    {
        var request = new LoginUserRequest("email@test.com", "123");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "User",
            Email = "email@test.com",
            Password = "hashed",
            Interests = [],
            Role = "User"
        };

        _userRepositoryMock.Setup(r => r.GetByIdEmail(request.Email)).ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.VerifyPassword(user, request.Password)).Returns(false);

        await Assert.ThrowsAsync<Exception>(() => _service.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_ReturnsToken_WhenSuccess()
    {
        var request = new LoginUserRequest("email@test.com", "123");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "User",
            Email = "email@test.com",
            Password = "hashed",
            Interests = [],
            Role = "User"
        };

        _userRepositoryMock.Setup(r => r.GetByIdEmail(request.Email)).ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.VerifyPassword(user, request.Password)).Returns(true);
        _jwtServiceMock.Setup(j => j.GenerateToken(user)).Returns("token123");

        var result = await _service.LoginAsync(request);

        Assert.Equal("token123", result.Token);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsUpdatedUser()
    {
        var userId = Guid.NewGuid();
        var originalUser = new User
        {
            Id = userId,
            Name = "Old Name",
            Email = "old@test.com",
            Password = "oldpass",
            Interests = ["Tech","AI"],
            Role = "User"
        };

        var updateRequest = new UpdateUserRequest("New Name", "new@test.com", ["Tecnologia"], "12345678","Admin");

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(originalUser);
        _passwordHasherMock.Setup(p => p.HashPassword(originalUser, "newpass")).Returns("newhashed");
        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        var result = await _service.UpdateAsync(userId, updateRequest);

        Assert.NotNull(result);
        Assert.Equal("New Name", result!.Name);
        Assert.Contains("Tech", result.Interests);
        Assert.Contains("AI", result.Interests);
        Assert.Equal("Admin", result.Role);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue()
    {
        var id = Guid.NewGuid();

        _userRepositoryMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(id);

        Assert.True(result);
    }
}
