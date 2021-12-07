namespace CinemaKeeper.Persistence;

public interface IDbContextCreator<TContext> where TContext : IDbContext
{
    TContext Create();
}
