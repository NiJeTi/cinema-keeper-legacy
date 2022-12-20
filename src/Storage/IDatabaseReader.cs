using System.Collections.ObjectModel;

using CinemaKeeper.Storage.Models;

namespace CinemaKeeper.Storage;

public interface IDatabaseReader
{
    ReadOnlyCollection<Quote> GetUserQuotes(ulong userId, ulong guildId);
}
