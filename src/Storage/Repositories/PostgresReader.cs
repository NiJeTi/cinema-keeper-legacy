using System.Collections.ObjectModel;

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

    public ReadOnlyCollection<Quote> GetUserQuotes(ulong userId, ulong guildId) => GetUserQuotesAsync(userId, guildId).GetAwaiter().GetResult();

    public async Task<ReadOnlyCollection<Quote>> GetUserQuotesAsync(ulong userId, ulong guildId)
    {
        var quotes = await _postgres.Quotes
           .Where(q => q.Author == userId && q.CreatedOn == guildId)
           .OrderBy(q => q.CreatedAt)
           .ToListAsync();

        return quotes.AsReadOnly();
    }
}
