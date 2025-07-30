using System.Data;
using Dapper;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;

namespace Newsletter.Infrastructure.Repository;

public class NewsletterRepository : INewsletterRepository
{
    private readonly IDbConnection _connection;

    public NewsletterRepository(IDatabaseContext databaseContext)
    {
        _connection = databaseContext.CreateConnection();
    }

    public async Task<IEnumerable<NewsletterEntry>> GetByUserIdAsync(Guid userId)
    {
        var sql =
            @"
            SELECT id, user_id, topics, content, sent, created_at
            FROM newsletter_entries
            WHERE user_id = @UserId";

        return await _connection.QueryAsync<NewsletterEntry>(sql, new { UserId = userId });
    }

    public async Task<NewsletterEntry> GetByIdAsync(Guid userId, Guid newsletterId)
    {
        var sql =
            @"
            SELECT id, user_id, topics, content, sent, created_at
            FROM newsletter_entries
            WHERE id = @NewsletterId AND user_id = @UserId";

        return await _connection.QuerySingleOrDefaultAsync<NewsletterEntry>(
            sql,
            new { UserId = userId, NewsletterId = newsletterId }
        );
    }

    public async Task<NewsletterEntry> GenerateAndSendAsync(NewsletterEntry newsletter)
    {
        var sql =
            @"
            INSERT INTO newsletter_entries (id, user_id, topics, content, sent, created_at)
            VALUES (@Id, @UserId, @Topics, @Content, @Sent, @CreatedAt)
            RETURNING id, user_id, topics, content, sent, created_at";

        return await _connection.QuerySingleAsync<NewsletterEntry>(
            sql,
            new
            {
                newsletter.Id,
                newsletter.UserId,
                Topics = newsletter.Topics.ToArray(),
                newsletter.Content,
                newsletter.Sent,
                newsletter.CreatedAt,
            }
        );
    }

    public async Task<bool> DeleteAsync(Guid userId)
    {
        var sql = @"DELETE FROM newsletter_entries WHERE user_id = @UserId";
        var affected = await _connection.ExecuteAsync(sql, new { UserId = userId });
        return affected > 0;
    }
}
