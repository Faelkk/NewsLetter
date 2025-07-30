using Newsletter.Domain.Entities;

namespace Newsletter.Domain.Interfaces;


public interface INewsletterRepository
{
    Task<IEnumerable<NewsletterEntry>> GetByUserIdAsync(Guid userId);

    Task<NewsletterEntry> GetByIdAsync(Guid userId, Guid newsletterId);
    
    Task<NewsletterEntry> GenerateAndSendAsync(NewsletterEntry newsletter);

    Task<bool> DeleteAsync(Guid userId);

    Task<bool> DeleteAsyncById(Guid userId, Guid newsletterId);
}

