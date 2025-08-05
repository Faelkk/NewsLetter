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
        var sql = "SELECT * FROM Subscriptions WHERE user_id = @userId";
        return await _connection.QueryFirstOrDefaultAsync<Subscription>(sql, new { userId });
    }
    
    public async Task<Subscription?> GetBySubscriptionIdAsync(Guid Id)
    {
        var sql = "SELECT * FROM Subscriptions WHERE id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<Subscription>(sql, new { Id });
    }

    public async Task<Subscription> CreateAsync(Subscription subscription)
    {
        if (subscription.Id == Guid.Empty)
            subscription.Id = Guid.NewGuid();

        subscription.UpdatedAt = DateTime.UtcNow;

        var sql = @"
        INSERT INTO Subscriptions 
            (id, user_id, external_subscription_id,plan, provider, status, started_at, expires_at, canceled_at, updated_at)
        VALUES 
            (@Id, @UserId, @ExternalSubscriptionId,@Plan, @Provider, @Status, @StartedAt, @ExpiresAt, @CanceledAt, @UpdatedAt)
        RETURNING *;";

        var createdSubscription = await _connection.QuerySingleAsync<Subscription>(sql, subscription);
        return createdSubscription;
    }


    public async Task<Subscription?> UpdateAsync(Subscription subscription)
    {
        subscription.UpdatedAt = DateTime.UtcNow;

        var sql = @"
        UPDATE Subscriptions SET
            external_subscription_id = @ExternalSubscriptionId,
            plan = @Plan,
            provider = @Provider,
            status = @Status,
            started_at = @StartedAt,
            expires_at = @ExpiresAt,
            canceled_at = @CanceledAt,
            updated_at = @UpdatedAt
        WHERE id = @Id
        RETURNING *;";

        var updatedSubscription = await _connection.QuerySingleOrDefaultAsync<Subscription>(sql, subscription);
        return updatedSubscription;
    }


    public async Task<bool> DeleteAsync(Guid userId)
    {
        var sql = "DELETE FROM Subscriptions WHERE user_id = @userId";
        var affectedRows = await _connection.ExecuteAsync(sql, new { userId });
        return affectedRows > 0;
    }
}



