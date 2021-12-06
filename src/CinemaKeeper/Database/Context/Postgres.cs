using Microsoft.EntityFrameworkCore;

using Serilog;

namespace CinemaKeeper.Database.Context;

public class Postgres : DbContext
{
    private readonly ILogger _logger;

    public Postgres(DbContextOptions<Postgres> options, ILogger logger) : base(options)
    {
        _logger = logger.ForContext<Postgres>();

        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
           .LogTo(_logger.Debug);
    }
}
