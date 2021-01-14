namespace CinemaKeeper.Service.Adapters
{
    internal interface IAdapter<out T>
    {
        T Convert();
    }
}
