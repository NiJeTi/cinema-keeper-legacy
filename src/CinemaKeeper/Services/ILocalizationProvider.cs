namespace CinemaKeeper.Services;

public interface ILocalizationProvider
{
    string Get(string key);
    string Get(string key, params object?[] args);
}
