using CinemaKeeper.Database.Models;

using Microsoft.EntityFrameworkCore;

using Serilog;

namespace CinemaKeeper.Database.Context;

public sealed class Postgres : DbContext
{
    private readonly ILogger _logger;

    public Postgres(DbContextOptions<Postgres> options, ILogger logger) : base(options)
    {
        _logger = logger.ForContext<Postgres>();

        Database.Migrate();
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<Quote> Quotes => Set<Quote>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(_logger.Verbose);
    }
}
