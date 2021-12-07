using Microsoft.EntityFrameworkCore;

namespace CinemaKeeper.Persistence;

public class DbContextBase : DbContext, IDbContext
{
    private readonly string _connectionString;
    private readonly int _commandTimeout;

    protected DbContextBase(string connectionString, int commandTimeout)
    {
        _connectionString = connectionString;
        _commandTimeout = commandTimeout;
    }

    protected DbContextBase(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_connectionString, options => options.CommandTimeout(_commandTimeout));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
