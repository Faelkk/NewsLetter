namespace NewsLetter.Test.Test.Fixtures;

using Microsoft.Extensions.Configuration;
using Newsletter.Infrastructure.Context;

public class TestDatabaseContext : DatabaseContext
{
    public TestDatabaseContext(string connectionString)
        : base(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", connectionString }
            })
            .Build())
    {
    }
}
