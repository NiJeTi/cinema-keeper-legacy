using System;
using System.Threading.Tasks;

namespace CinemaKeeper.Service.Helpers
{
    public interface IExceptionShield<in T>
    {
        Task Protect(T context, Func<Task> action);
    }
}
