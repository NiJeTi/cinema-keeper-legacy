using System;

namespace CinemaKeeper.Exceptions;

public class LocalizationException : Exception
{
    public LocalizationException(string? message) : base(message) { }
}
