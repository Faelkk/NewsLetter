using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Newsletter.Infrastructure.Context;

public class DatabaseContext : IDatabaseContext
{
    private readonly string? _connectionString;

    public DatabaseContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}