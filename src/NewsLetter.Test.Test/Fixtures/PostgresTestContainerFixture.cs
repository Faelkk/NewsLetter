using Microsoft.Extensions.Configuration;
using Npgsql;
using Dapper;
using Testcontainers.PostgreSql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsLetter.Test.Test.Fixtures;

public class PostgresTestContainerFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; private set; } = null!;
    public string ConnectionString => Container.GetConnectionString();

    private IConfiguration? _configuration;

    public IConfiguration Configuration => _configuration ??= new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", ConnectionString }
        })
        .Build();

    public async Task InitializeAsync()
    {
        Container = new PostgreSqlBuilder()
            .WithDatabase("newsletter_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await Container.StartAsync();
        await ApplyMigrations();
    }

    private async Task ApplyMigrations()
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        var sql = @"
            CREATE TABLE IF NOT EXISTS Users (
                id UUID PRIMARY KEY,
                name TEXT NOT NULL,
                email TEXT NOT NULL,
                password TEXT NOT NULL,
                role TEXT NOT NULL,
                interests TEXT[] NOT NULL
            );

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

            CREATE TABLE IF NOT EXISTS Newsletters (
                id UUID PRIMARY KEY,
                user_id UUID NOT NULL REFERENCES Users(id) ON DELETE CASCADE,
                topics TEXT[] NOT NULL,
                content TEXT NOT NULL,
                sent BOOLEAN NOT NULL,
                created_at TIMESTAMP NOT NULL
            );
        ";

        await conn.ExecuteAsync(sql);
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
    }
    
    public async Task ResetDatabase()
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();
        var sql = @"
        TRUNCATE TABLE Newsletters RESTART IDENTITY CASCADE;
        TRUNCATE TABLE Subscriptions RESTART IDENTITY CASCADE;
        TRUNCATE TABLE Users RESTART IDENTITY CASCADE;
    ";
        await conn.ExecuteAsync(sql);
    }
}
