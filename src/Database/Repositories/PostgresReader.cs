using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using CinemaKeeper.Database.Context;
using CinemaKeeper.Database.Models;

using Microsoft.EntityFrameworkCore;

namespace CinemaKeeper.Database.Repositories;

public class PostgresReader : IDatabaseReader, IAsyncDatabaseReader
{
    private readonly Postgres _postgres;

    public PostgresReader(Postgres postgres)
    {
        _postgres = postgres;
    }

    public ReadOnlyCollection<Quote> GetUserQuotes(ulong userId) =>
        _postgres.Quotes
           .Where(q => q.Author == userId)
           .OrderBy(q => q.CreatedAt)
           .ToList()
           .AsReadOnly();

    public async Task<ReadOnlyCollection<Quote>> GetUserQuotesAsync(ulong userId)
    {
        var quotes = await _postgres.Quotes
           .Where(q => q.Author == userId)
           .ToListAsync();

        return quotes.AsReadOnly();
    }
}
