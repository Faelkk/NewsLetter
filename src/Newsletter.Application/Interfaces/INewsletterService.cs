using Newsletter.Presentation.DTOS;

namespace Newsletter.Application.Interfaces;

public interface INewsletterService
{
    Task<IEnumerable<NewsletterDto>> GetByUserIdAsync(Guid userId);

    Task<NewsletterDto> GetByIdAsync(Guid userId, Guid newsletterId);

    Task<NewsletterDto> GenerateAndSendAsync(GenerateNewsletterRequest request);

    Task<bool> DeleteAsync(Guid userId);
}