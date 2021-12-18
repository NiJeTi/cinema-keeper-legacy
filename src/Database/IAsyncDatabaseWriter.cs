using System.Threading.Tasks;

using CinemaKeeper.Database.Models;

namespace CinemaKeeper.Database;

public interface IAsyncDatabaseWriter
{
    Task AddQuoteAsync(Quote quote);
}
