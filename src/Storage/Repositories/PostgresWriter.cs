using System.Threading.Tasks;

using CinemaKeeper.Storage.Context;
using CinemaKeeper.Storage.Models;

namespace CinemaKeeper.Storage.Repositories;

public class PostgresWriter : IDatabaseWriter, IAsyncDatabaseWriter
{
    private readonly Postgres _postgres;

    public PostgresWriter(Postgres postgres)
    {
        _postgres = postgres;
    }

    public void AddQuote(Quote quote)
    {
        _postgres.Quotes.Add(quote);
        _postgres.SaveChanges();
    }

    public async Task AddQuoteAsync(Quote quote)
    {
        await _postgres.Quotes.AddAsync(quote);
        await _postgres.SaveChangesAsync();
    }
}
