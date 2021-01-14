using System;
using System.Threading.Tasks;

namespace CinemaKeeper.Service.Helpers
{
    internal interface IExceptionShield<in T>
    {
        Task Protect(T context, Func<Task> action);
    }
}
