namespace NewsLetter.Test.Test.Fixtures;

using Xunit;

public abstract class DatabaseTestBase : IAsyncLifetime
{
    private readonly PostgresTestContainerFixture _fixture;

    protected DatabaseTestBase(PostgresTestContainerFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabase();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
