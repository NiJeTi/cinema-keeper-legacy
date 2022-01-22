using CinemaKeeper.Storage.Models;

using Microsoft.EntityFrameworkCore;

using Serilog;

namespace CinemaKeeper.Storage.Context;

public sealed class Postgres : DbContext
{
    private static bool _isActual;

    private readonly ILogger _logger;

    public Postgres(DbContextOptions<Postgres> options, ILogger logger) : base(options)
    {
        _logger = logger.ForContext<Postgres>();

        if (!_isActual)
        {
            Database.Migrate();
            _isActual = true;
        }

        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<Quote> Quotes => Set<Quote>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(_logger.Debug);
    }
}
