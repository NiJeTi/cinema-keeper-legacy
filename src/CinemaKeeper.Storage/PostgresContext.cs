using CinemaKeeper.Storage.Models;

using Microsoft.EntityFrameworkCore;

namespace CinemaKeeper.Storage;

public sealed class PostgresContext : DbContext
{
    public PostgresContext(DbContextOptions<PostgresContext> options) : base(options)
    {
        Quotes = Set<Quote>();
    }

    public DbSet<Quote> Quotes { get; }
}
