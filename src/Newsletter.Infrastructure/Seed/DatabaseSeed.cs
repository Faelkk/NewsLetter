using System.Data;
using Dapper;
using Newsletter.Domain.Entities;
using Newsletter.Infrastructure.Context;

namespace Newsletter.Infrastructure.Seed;

public class DatabaseSeed
{
    private readonly IDatabaseContext _context;

    public DatabaseSeed(IDatabaseContext context)
    {
        _context = context;
    }

    public void Initialize()
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        CreateTables(connection);
    }

    private void CreateTables(IDbConnection connection)
    {
        var createUsers = """
                          CREATE TABLE IF NOT EXISTS Users (
                              id UUID PRIMARY KEY,
                              name TEXT NOT NULL,
                              email TEXT NOT NULL UNIQUE,
                              plan TEXT NOT NULL,
                              interests TEXT[] NOT NULL
                          );
                          """;

        var createEmailIndex = """
                               CREATE UNIQUE INDEX IF NOT EXISTS idx_users_email ON users(email);
                               """;

        var createSubscriptions = """
                                  CREATE TABLE IF NOT EXISTS Subscriptions (
                                      id UUID PRIMARY KEY,
                                      user_id UUID NOT NULL,
                                      external_subscription_id TEXT NOT NULL,
                                      provider TEXT NOT NULL,
                                      status TEXT NOT NULL,
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
                                    user_id UUID NOT NULL,
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

        var tables = connection.Query<string>(
            """
            SELECT table_name
            FROM information_schema.tables
            WHERE table_schema = 'public'
            ORDER BY table_name;
            """);

        Console.WriteLine("🧩 Tabelas existentes no schema 'public':");
        foreach (var table in tables)
        {
            Console.WriteLine($"✅ {table}");
        }
    }
}

