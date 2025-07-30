

using System.Data;

public interface IDatabaseContext
{
    IDbConnection CreateConnection();
}