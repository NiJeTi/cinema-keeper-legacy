using System.Collections.ObjectModel;

using CinemaKeeper.Storage.Models;

namespace CinemaKeeper.Storage;

public interface IAsyncDatabaseReader
{
    Task<ReadOnlyCollection<Quote>> GetUserQuotesAsync(ulong userId, ulong guildId);
}
