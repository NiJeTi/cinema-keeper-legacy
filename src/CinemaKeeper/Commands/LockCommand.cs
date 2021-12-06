using System.Threading.Tasks;

using CinemaKeeper.Commands.Preconditions;
using CinemaKeeper.Exceptions;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Serilog;

namespace CinemaKeeper.Commands;

[RequireContext(ContextType.Guild)]
[RequireBotPermission(GuildPermission.ManageChannels)]
[RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
[UserMustBeInVoiceChannel]
public class LockCommand : ModuleBase<SocketCommandContext>
{
    private readonly ILogger _logger;

    public LockCommand(ILogger logger)
    {
        _logger = logger.ForContext<LockCommand>();
    }

    [Command("lock")]
    public async Task Lock()
    {
        var voiceChannel = ((SocketGuildUser) Context.User).VoiceChannel;

        await voiceChannel.ModifyAsync(vcp => vcp.UserLimit = voiceChannel.Users.Count);

        _logger.Debug("Locked channel {VoiceChannel} for {UserLimit} user(s)", voiceChannel.Name,
            voiceChannel.UserLimit);
    }

    [Command("lock")]
    public async Task Lock(int userLimit)
    {
        var voiceChannel = ((SocketGuildUser) Context.User).VoiceChannel;

        // TODO: Find a way to use PreconditionAttribute
        if (!IsUsersLimitValid(userLimit, voiceChannel.Users.Count))
            throw new InvalidUserLimitException();

        await voiceChannel.ModifyAsync(vcp => vcp.UserLimit = userLimit);

        _logger.Debug("Locked channel {VoiceChannel} for {UserLimit} user(s)", voiceChannel.Name,
            voiceChannel.UserLimit);
    }

    private static bool IsUsersLimitValid(int value, int currentUsersCount)
    {
        const int minUsersLimit = 1;
        const int maxUsersLimit = 99;

        return value >= currentUsersCount
            && value is >= minUsersLimit and <= maxUsersLimit;
    }
}
