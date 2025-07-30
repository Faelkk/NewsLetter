using Newsletter.Application.Interfaces;
using Newsletter.Presentation.DTOS;

namespace Newsletter.Application.Services;

public class NewsletterService : INewsletterService
{
    public Task<NewsletterDto> GetByUserIdAsync(Guid userId)
    {
        var newsletter = new NewsletterDto(
            Id: Guid.NewGuid(),
            UserId: userId,
            Topics: new List<string> { "IA", "Startups" },
            Content: "Aqui está sua newsletter personalizada sobre IA e Startups!",
            Sent: true,
            CreatedAt: DateTime.UtcNow.AddDays(-1)
        );

        return Task.FromResult(newsletter);
    }

    public Task<NewsletterDto> GetByIdAsync(Guid userId, Guid newsletterId)
    {
        var newsletter = new NewsletterDto(
            Id: newsletterId,
            UserId: userId,
            Topics: new List<string> { "Tecnologia", "Negócios" },
            Content: "Conteúdo gerado para sua newsletter.",
            Sent: true,
            CreatedAt: DateTime.UtcNow.AddDays(-3)
        );

        return Task.FromResult(newsletter);
    }

    public Task<NewsletterDto> GenerateAndSendAsync(GenerateNewsletterRequest request)
    {
        var generated = new NewsletterDto(
            Id: Guid.NewGuid(),
            UserId: request.UserId,
            Topics: request.Topics,
            Content: $"Conteúdo gerado automaticamente sobre: {string.Join(", ", request.Topics)}.",
            Sent: true,
            CreatedAt: DateTime.UtcNow
        );

        return Task.FromResult(generated);
    }

    public Task<bool> DeleteAsync(Guid userId)
    {
        // Simula a exclusão com sucesso
        return Task.FromResult(true);
    }
}