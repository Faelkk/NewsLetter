using Newsletter.Application.Interfaces;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;
using Newsletter.Presentation.DTOS;

namespace Newsletter.Application.Services;

public class NewsletterService : INewsletterService
{
    private readonly INewsletterRepository _newsLetterRepository;
    private readonly IUserRepository _userRepository;

    public NewsletterService(INewsletterRepository repository,IUserRepository userRepository)
    {
        _newsLetterRepository = repository;
        _userRepository = userRepository;
    }
    

    public async Task<IEnumerable<NewsletterDto>> GetByUserIdAsync(Guid userId)
    {
        var newsletters = await _newsLetterRepository.GetByUserIdAsync(userId);
        return newsletters.Select(MapToDto);
    }

    public async Task<NewsletterDto> GetByIdAsync(Guid userId, Guid newsletterId)
    {
        var newsletter = await _newsLetterRepository.GetByIdAsync(userId, newsletterId);
        if (newsletter is null)
            throw new Exception("Newsletter não encontrada.");

        return MapToDto(newsletter);
    }

    public async Task<NewsletterDto> GenerateAndSendAsync(GenerateNewsletterRequest request)
    {
     
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null)
            throw new Exception("Usuário não encontrado.");

        if (user.Interests == null || user.Interests.Length == 0)

            throw new Exception("Usuário não possui interesses cadastrados.");

        var topics = user.Interests;

        
        var entity = new NewsletterEntry
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Topics = topics,
            Content = $"Conteúdo gerado automaticamente sobre: {string.Join(", ", topics)}.",
            Sent = true,
            CreatedAt = DateTime.UtcNow
        };

        var saved = await _newsLetterRepository.GenerateAndSendAsync(entity);
        return MapToDto(saved);
    }


    public async Task<bool> DeleteAsync(Guid userId)
    {
        return await _newsLetterRepository.DeleteAsync(userId);
    }
    public async Task<bool> DeleteAsyncNewLetterId(Guid userId, Guid newsletterId)
    {
        return await _newsLetterRepository.DeleteAsyncById(userId,newsletterId);
    }


    private static NewsletterDto MapToDto(NewsletterEntry entity)
    {
        return new NewsletterDto(
            Id: entity.Id,
            UserId: entity.UserId,
            Topics: entity.Topics.ToList(),
            Content: entity.Content,
            Sent: entity.Sent,
            CreatedAt: entity.CreatedAt
        );
    }
}