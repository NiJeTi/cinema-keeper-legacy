namespace CinemaKeeper.Exceptions;

public class UserNotInVoiceChannelException : FlowException
{
    public UserNotInVoiceChannelException() : base("errors.userMustBeInVoiceChannel") { }
}
