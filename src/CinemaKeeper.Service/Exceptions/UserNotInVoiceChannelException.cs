using System;

namespace CinemaKeeper.Service.Exceptions
{
    public class UserNotInVoiceChannelException : Exception
    {
        public UserNotInVoiceChannelException() { }

        public UserNotInVoiceChannelException(string? message) : base(message) { }

        public UserNotInVoiceChannelException(string? message, Exception? innerException) : base(message,
            innerException) { }
    }
}
