using System;

namespace CinemaKeeper.Exceptions;

public class FlowException : Exception
{
    public FlowException(string localizationKey) : base(localizationKey) { }
}
