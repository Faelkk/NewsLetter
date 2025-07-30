using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Newsletter.Infrastructure.Context;

public class DatabaseContext : IDatabaseContext
{
    private readonly string? _connectionString;

    public DatabaseContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new ArgumentNullException("Connection string is missing.");
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}

