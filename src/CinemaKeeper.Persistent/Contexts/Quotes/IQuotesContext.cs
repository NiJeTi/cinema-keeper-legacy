using System.Linq;

using CinemaKeeper.Persistence.Models;

namespace CinemaKeeper.Persistence.Contexts.Quotes;

public interface IQuotesContext : IDbContext
{
    IQueryable<Quote> Quotes { get; }

    void SaveQuote(Quote quote);
}
