using System.Data;
using Dapper;
using Newsletter.Application.Interfaces;
using Newsletter.Domain.Entities;
using Newsletter.Infrastructure.Context;

namespace Newsletter.Infrastructure.Seed
{
    public class DatabaseSeed
    {
        private readonly IDatabaseContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public DatabaseSeed(IDatabaseContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public void Initialize()
        {
            using var connection = _context.CreateConnection();
            connection.Open();

            CreateTables(connection);
            InsertAdminUser(connection);
        }

        private void CreateTables(IDbConnection connection)
{
    
    
    var createUsers = """
        CREATE TABLE IF NOT EXISTS Users (
                   id UUID PRIMARY KEY,
                   name TEXT NOT NULL,
                   email TEXT NOT NULL,
                   password TEXT NOT NULL,
                   role TEXT NOT NULL,
                   interests TEXT[] NOT NULL
               );
    """;

    var createEmailIndex = """
        CREATE UNIQUE INDEX IF NOT EXISTS idx_users_email ON users(email);
    """;
    
    
    var createSubscriptions = """
         CREATE TABLE IF NOT EXISTS Subscriptions (
                id UUID PRIMARY KEY,
                user_id UUID NOT NULL REFERENCES Users(id) ON DELETE CASCADE,
                external_subscription_id TEXT,
                provider TEXT NOT NULL,
                plan TEXT,
                status TEXT NOT NULL,
                next_delivery_date TIMESTAMP,
                started_at TIMESTAMP,
                expires_at TIMESTAMP,
                canceled_at TIMESTAMP,
                updated_at TIMESTAMP NOT NULL
            );
    """;

    var createSubscriptionUserIdUniqueIndex = """
        CREATE UNIQUE INDEX IF NOT EXISTS idx_subscriptions_user_id ON Subscriptions(user_id);
    """;

    var createNewsletters = """
        CREATE TABLE IF NOT EXISTS Newsletters (
        id UUID PRIMARY KEY,
        user_id UUID NOT NULL REFERENCES Users(id) ON DELETE CASCADE,
        topics TEXT[] NOT NULL,
        content TEXT NOT NULL,
        sent BOOLEAN NOT NULL,
        created_at TIMESTAMP NOT NULL
    );
    """;
    connection.Execute(createUsers);
    connection.Execute(createEmailIndex);
    connection.Execute(createSubscriptions);
    connection.Execute(createSubscriptionUserIdUniqueIndex);
    connection.Execute(createNewsletters);
}


        private void InsertAdminUser(IDbConnection connection)
        {
            var exists = connection.ExecuteScalar<bool>(
                "SELECT EXISTS(SELECT 1 FROM Users WHERE email = @Email)",
                new { Email = "admin@example.com" });

            if (!exists)
            {
                var tempUser = new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Email = "admin@example.com",
                    Interests = new[] { "tecnologia", "programação", "notícias" },
                    Role = "Admin",
                    Password = ""
                };

                var hashedPassword = _passwordHasher.HashPassword(tempUser, "admin123");

                var adminUser = new User
                {
                    Id = tempUser.Id,
                    Name = tempUser.Name,
                    Email = tempUser.Email,
                    Interests = tempUser.Interests,
                    Role = tempUser.Role,
                    Password = hashedPassword
                };

                var insertSql = """
                    INSERT INTO Users (id, name, email, password, interests, role)
                    VALUES (@Id, @Name, @Email, @Password, @Interests, @Role);
                """;
                
                
                connection.Execute(insertSql, new
                {
                    Id = adminUser.Id,
                    Name = adminUser.Name,
                    Email = adminUser.Email,
                    Password = adminUser.Password,
                    Interests = adminUser.Interests,
                    Role = adminUser.Role
                });
            }
        }
    }
}
