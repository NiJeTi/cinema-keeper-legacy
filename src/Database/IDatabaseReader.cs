using System.Collections.ObjectModel;

using CinemaKeeper.Database.Models;

namespace CinemaKeeper.Database;

public interface IDatabaseReader
{
    ReadOnlyCollection<Quote> GetUserQuotes(ulong userId);
}
