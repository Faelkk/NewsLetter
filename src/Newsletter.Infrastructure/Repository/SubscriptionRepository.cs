using System.Data;
using Dapper;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;

namespace Newsletter.Infrastructure.Repository;

public class SubscriptionRepository : ISubscriptionRepository
{ 
    private readonly IDbConnection _connection;

    public SubscriptionRepository(IDatabaseContext databaseContext) 
    {
        _connection = databaseContext.CreateConnection();
    }

    public async Task<IEnumerable<Subscription>> GetAllAsync()
    {
        var sql = "SELECT * FROM Subscriptions";
        return await _connection.QueryAsync<Subscription>(sql);
    }

    public async Task<Subscription?> GetByUserIdAsync(Guid userId)
    {
        var sql = "SELECT * FROM Subscriptions WHERE UserId = @userId";
        return await _connection.QueryFirstOrDefaultAsync<Subscription>(sql, new { userId });
    }

    public async Task<Subscription> CreateAsync(Subscription subscription)
    {
        var sql = @"
            INSERT INTO Subscriptions 
                (Id, UserId, ExternalSubscriptionId, Provider, Status, StartedAt, ExpiresAt, CanceledAt, UpdatedAt)
            VALUES 
                (@Id, @UserId, @ExternalSubscriptionId, @Provider, @Status, @StartedAt, @ExpiresAt, @CanceledAt, @UpdatedAt);
        ";


        if (subscription.Id == Guid.Empty)
            subscription.Id = Guid.NewGuid();


        subscription.UpdatedAt = DateTime.UtcNow;

        await _connection.ExecuteAsync(sql, subscription);

        return subscription;
    }

    public async Task<Subscription?> UpdateAsync(Subscription subscription)
    {
        var sql = @"
            UPDATE Subscriptions SET
                ExternalSubscriptionId = @ExternalSubscriptionId,
                Provider = @Provider,
                Status = @Status,
                StartedAt = @StartedAt,
                ExpiresAt = @ExpiresAt,
                CanceledAt = @CanceledAt,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id;
        ";

        subscription.UpdatedAt = DateTime.UtcNow;

        var affectedRows = await _connection.ExecuteAsync(sql, subscription);

        if (affectedRows == 0)
            return null;

        return subscription;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var sql = "DELETE FROM Subscriptions WHERE Id = @id";
        var affectedRows = await _connection.ExecuteAsync(sql, new { id });
        return affectedRows > 0;
    }
}
