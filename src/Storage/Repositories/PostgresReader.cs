using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using CinemaKeeper.Storage.Context;
using CinemaKeeper.Storage.Models;

using Microsoft.EntityFrameworkCore;

namespace CinemaKeeper.Storage.Repositories;

public class PostgresReader : IDatabaseReader, IAsyncDatabaseReader
{
    private readonly Postgres _postgres;

    public PostgresReader(Postgres postgres)
    {
        _postgres = postgres;
    }

    public ReadOnlyCollection<Quote> GetUserQuotes(ulong userId) => GetUserQuotesAsync(userId).GetAwaiter().GetResult();

    public async Task<ReadOnlyCollection<Quote>> GetUserQuotesAsync(ulong userId)
    {
        var quotes = await _postgres.Quotes
           .Where(q => q.Author == userId)
           .OrderBy(q => q.CreatedAt)
           .ToListAsync();

        return quotes.AsReadOnly();
    }
}
