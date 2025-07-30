using Newsletter.Application.Interfaces;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;
using Newsletter.Presentation.DTOS;

namespace Newsletter.Application.Services;

public class NewsletterService : INewsletterService
{
    private readonly INewsletterRepository _repository;

    public NewsletterService(INewsletterRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<NewsletterDto>> GetByUserIdAsync(Guid userId)
    {
        var newsletters = await _repository.GetByUserIdAsync(userId);
        return newsletters.Select(MapToDto);
    }

    public async Task<NewsletterDto> GetByIdAsync(Guid userId, Guid newsletterId)
    {
        var newsletter = await _repository.GetByIdAsync(userId, newsletterId);
        if (newsletter is null)
            throw new Exception("Newsletter não encontrada.");

        return MapToDto(newsletter);
    }

    public async Task<NewsletterDto> GenerateAndSendAsync(GenerateNewsletterRequest request)
    {
        var entity = new NewsletterEntry
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Topics = request.Topics,
            Content = $"Conteúdo gerado automaticamente sobre: {string.Join(", ", request.Topics)}.",
            Sent = true,
            CreatedAt = DateTime.UtcNow
        };

        var saved = await _repository.GenerateAndSendAsync(entity);
        return MapToDto(saved);
    }

    public async Task<bool> DeleteAsync(Guid userId)
    {
        return await _repository.DeleteAsync(userId);
    }

    private static NewsletterDto MapToDto(NewsletterEntry entity)
    {
        return new NewsletterDto(
            Id: entity.Id,
            UserId: entity.UserId,
            Topics: entity.Topics,
            Content: entity.Content,
            Sent: entity.Sent,
            CreatedAt: entity.CreatedAt
        );
    }
}