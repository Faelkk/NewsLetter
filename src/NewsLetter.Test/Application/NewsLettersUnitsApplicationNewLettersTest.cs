using Newsletter.Application.Services;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;
using Newsletter.Presentation.DTOS;
using Moq;

namespace NewsLetter.Test.Application;

public class NewsLettersUnitsApplicationNewLettersTest
    {
        private readonly Mock<INewsletterRepository> _newsletterRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly NewsletterService _service;

        public NewsLettersUnitsApplicationNewLettersTest()
        {
            _newsletterRepositoryMock = new Mock<INewsletterRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _service = new NewsletterService(_newsletterRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsNewsletters()
        {
            var userId = Guid.NewGuid();
            var newsletters = new List<NewsletterEntry>
            {
                new NewsletterEntry
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Topics = new[] { "Tech" },
                    Content = "Conteúdo gerado automaticamente sobre: Tech.",
                    Sent = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _newsletterRepositoryMock
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(newsletters);
            
            var result = await _service.GetByUserIdAsync(userId);

            Assert.Single(result);
            Assert.Equal("Tech", result.First().Topics.First());
        }

        [Fact]
        public async Task GetByIdAsync_Throws_WhenNotFound()
        {
            var userId = Guid.NewGuid();
            var newsletterId = Guid.NewGuid();

            _newsletterRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, newsletterId))
                .ReturnsAsync((NewsletterEntry?)null);
            
            await Assert.ThrowsAsync<Exception>(() =>
                _service.GetByIdAsync(userId, newsletterId));
        }

        [Fact]
        public async Task GenerateAndSendAsync_Throws_WhenUserNotFound()
        {
            var request = new GenerateNewsletterRequest
            (
                Guid.NewGuid()
            );

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(request.UserId))
                .ReturnsAsync((User?)null);

      
            await Assert.ThrowsAsync<Exception>(() =>
                _service.GenerateAndSendAsync(request));
        }

        [Fact]
        public async Task GenerateAndSendAsync_Throws_WhenUserHasNoInterests()
        {
     
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Rafael",
                Email = "rafael@example.com",
                Password = "123",
                Role = "User",
                Interests = Array.Empty<string>()
            };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            var request = new GenerateNewsletterRequest(user.Id);
            
            await Assert.ThrowsAsync<Exception>(() =>
                _service.GenerateAndSendAsync(request));
        }

        [Fact]
        public async Task GenerateAndSendAsync_ReturnsNewsletterDto()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Rafael",
                Email = "rafael@example.com",
                Password = "123",
                Role = "User",
                Interests = new[] { "Tech", "AI" }
            };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            _newsletterRepositoryMock
                .Setup(r => r.GenerateAndSendAsync(It.IsAny<NewsletterEntry>()))
                .ReturnsAsync((NewsletterEntry entry) => entry);

            var request = new GenerateNewsletterRequest(user.Id);
            
            var result = await _service.GenerateAndSendAsync(request);

            Assert.Equal(user.Id, result.UserId);
            Assert.Contains("Tech", result.Topics);
            Assert.True(result.Sent);
        }
    }

