using System;

using CinemaKeeper.Persistence.Models;

using Microsoft.EntityFrameworkCore;

namespace CinemaKeeper.Persistence;

public sealed class PostgresContext : DbContext
{
    public PostgresContext(DbContextOptions<PostgresContext> options) : base(options)
    {
        Quotes = Set<Quote>();
    }

    public DbSet<Quote> Quotes { get; }
}
