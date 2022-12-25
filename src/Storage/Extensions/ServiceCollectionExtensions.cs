using CinemaKeeper.Storage.Context;
using CinemaKeeper.Storage.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaKeeper.Storage.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigurePersistentStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Postgres>(
            options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("Postgres"));
            });

        services.AddScoped<IDatabaseReader, PostgresReader>();
        services.AddScoped<IAsyncDatabaseReader, PostgresReader>();

        services.AddScoped<IDatabaseWriter, PostgresWriter>();
        services.AddScoped<IAsyncDatabaseWriter, PostgresWriter>();
    }
}
