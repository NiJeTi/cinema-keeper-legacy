using Npgsql;

namespace CinemaKeeper.Persistence;

public abstract class DbContextCreator<TContext> : IDbContextCreator<TContext>
    where TContext : IDbContext
{
    private readonly string _connectionString;
    private readonly int _commandTimeout;

    public DbContextCreator(string connectionString, int commandTimeout)
    {
        _connectionString = connectionString;
        _commandTimeout = commandTimeout;
    }

    public TContext Create()
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(_connectionString);

        return Create(connectionStringBuilder.ConnectionString, _commandTimeout);
    }

    protected abstract TContext Create(string connectionString, int commandTimeout);
}
