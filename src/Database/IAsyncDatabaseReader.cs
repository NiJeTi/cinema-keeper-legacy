using System.Collections.ObjectModel;
using System.Threading.Tasks;

using CinemaKeeper.Database.Models;

namespace CinemaKeeper.Database;

public interface IAsyncDatabaseReader
{
    Task<ReadOnlyCollection<Quote>> GetUserQuotesAsync(ulong userId);
}
