using System.Data;
using Dapper;
using Newsletter.Application.DTOS.Users;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;


namespace Newsletter.Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _connection;

    public UserRepository(IDatabaseContext databaseContext)
    {
        _connection = databaseContext.CreateConnection();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var sql = "SELECT * FROM Users";

        return await _connection.QueryAsync<User>(sql);
    }

    public async Task <User?> GetByIdAsync(Guid id)
    {
        var sql = "SELECT * FROM Users WHERE Id = @id";
        
        return await _connection.QueryFirstOrDefaultAsync<User>(sql, new { id });
        
    }

    public async Task<User?> GetByIdEmail(string email)
    {
        var sql = "SELECT * FROM Users WHERE Email = @email";
        
        return await _connection.QueryFirstOrDefaultAsync<User>(sql, new { email });
    }

    public async Task<User> CreateAsync(User user)
    {
        var sql = @"
        INSERT INTO Users (id, name, email, password, interests)
        VALUES (@Id, @Name, @Email, @Password, @Interests)
        RETURNING *;
    ";
        

        var createdUser = await _connection.QuerySingleAsync<User>(sql, user);
        return createdUser;
    }

    public async Task<User> UpdateAsync(User user)
    {
        var sql = @"
        UPDATE Users
        SET name = @Name,
            email = @Email,
            plan = @Plan,
            password = @Password,
            interests = @Interests
        WHERE id = @Id
        RETURNING *;
    ";

        var updatedUser = await _connection.QuerySingleOrDefaultAsync<User>(sql, user);
        return updatedUser!;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var sql = "DELETE FROM Users WHERE id = @Id";
        var rows = await _connection.ExecuteAsync(sql, new { Id = id });

        return rows > 0;
    }
}


