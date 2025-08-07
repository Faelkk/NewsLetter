using System.Data;
using Dapper;
using Newsletter.Domain.Dtos;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;
using Newsletter.Presentation.DTOS;

namespace Newsletter.Infrastructure.Repository;

public class SubscriptionRepository : ISubscriptionRepository
{ 
    private readonly IDbConnection _connection;

    public SubscriptionRepository(IDatabaseContext databaseContext) 
    {
        _connection = databaseContext.CreateConnection();
    }
    
    public async Task<IEnumerable<(Guid Id, DateTime? NextDeliveryDate)>> GetNextDeliveryDatesRawAsync()
    {
        var sql = @"
        SELECT id, next_delivery_date
        FROM Subscriptions
        ORDER BY next_delivery_date";

        var result = await _connection.QueryAsync<(Guid, DateTime?)>(sql);
        return result;
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
    
    public async Task<IEnumerable<SubscriptionWithUserEmail>> GetActiveSubscriptionsWithUserEmail(DateTime deliveryDate)
    {
        var startOfDay = deliveryDate.Date;
        var startOfNextDay = startOfDay.AddDays(1);

        var sql = @"
    SELECT 
        s.id AS SubscriptionId, 
        s.user_id AS UserId, 
        u.email AS UserEmail, 
        u.interests AS Interests,
        s.next_delivery_date AS NextDeliveryDate
    FROM Subscriptions s
    INNER JOIN Users u ON s.user_id = u.id
    WHERE s.status = 'Active'
      AND s.next_delivery_date >= @startOfDay
      AND s.next_delivery_date < @startOfNextDay";

        return await _connection.QueryAsync<SubscriptionWithUserEmail>(sql, new { startOfDay, startOfNextDay });
    }



    
    public async Task<Subscription?> GetBySubscriptionIdAsync(Guid Id)
    {
        var sql = "SELECT * FROM Subscriptions WHERE id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<Subscription>(sql, new { Id });
    }
    

    public async Task<Subscription?> GetBySubscriptionIdAndUserIdAsync(Guid subscriptionId, Guid userId)
    {
        var sql = "SELECT * FROM Subscriptions WHERE id = @subscriptionId AND user_id = @userId";
        return await _connection.QueryFirstOrDefaultAsync<Subscription>(sql, new { subscriptionId, userId });
    }




    public async Task<Subscription> CreateAsync(Subscription subscription)
    {
        if (subscription.Id == Guid.Empty)
            subscription.Id = Guid.NewGuid();

        subscription.UpdatedAt = DateTime.UtcNow;

        var sql = @"
        INSERT INTO Subscriptions 
            (id, user_id, external_subscription_id,plan, provider, status,next_delivery_date, started_at, expires_at, canceled_at, updated_at)
        VALUES 
            (@Id, @UserId, @ExternalSubscriptionId,@Plan, @Provider, @Status,@NextDeliveryDate, @StartedAt, @ExpiresAt, @CanceledAt, @UpdatedAt)
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
        next_delivery_date = @NextDeliveryDate, 
        started_at = @StartedAt,
        expires_at = @ExpiresAt,
        canceled_at = @CanceledAt,
        updated_at = @UpdatedAt
    WHERE id = @Id
    RETURNING *;";

        var updatedSubscription = await _connection.QuerySingleOrDefaultAsync<Subscription>(sql, subscription);
        return updatedSubscription;
    }

    
    public async Task UpdateNextDeliveryDateAsync(Guid id, DateTime? nextDeliveryDate)
    {
        var sql = @"
    UPDATE Subscriptions SET
        next_delivery_date = @NextDeliveryDate,
        updated_at = @UpdatedAt
    WHERE id = @Id";

        await _connection.ExecuteAsync(sql, new {
            Id = id,
            NextDeliveryDate = nextDeliveryDate,
            UpdatedAt = DateTime.UtcNow
        });
    }

    public async Task SetPendingStatusForExpiredSubscriptionsAsync(DateTime today)
    {
        var sql = @"
        UPDATE subscriptions
        SET status = 'pending',
            updated_at = @now
        WHERE status = 'active' AND expires_at <= @today";

        await _connection.ExecuteAsync(sql, new {
            today,
            now = DateTime.UtcNow
        });
    }



    public async Task<bool> DeleteAsync(Guid userId)
    {
        var sql = "DELETE FROM Subscriptions WHERE user_id = @userId";
        var affectedRows = await _connection.ExecuteAsync(sql, new { userId });
        return affectedRows > 0;
    }
}



 