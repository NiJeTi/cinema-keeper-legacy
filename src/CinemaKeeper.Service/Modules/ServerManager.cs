using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using CinemaKeeper.Service.Exceptions;
using CinemaKeeper.Service.Helpers;
using CinemaKeeper.Service.Configurations;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using RestSharp;
using Serilog;

namespace CinemaKeeper.Service.Modules
{

    public enum TypeCommand
    {
        start,
        reboot,
        stop,
        status,
        details,
        balans,
    }

    public class ServerManager : ModuleBase<SocketCommandContext>
    {


        private const string baseUrl = "https://api.cloudvps.reg.ru/v1/reglets/";
        private DiscordBotConfiguration Conf;
        private readonly IExceptionShield<SocketCommandContext> _shield;

        public ServerManager(IExceptionShield<SocketCommandContext> shield, DiscordBotConfiguration configuration)
        {
            Conf = configuration;
            _shield = shield;
        }






        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        [RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
        [Command("server")]

        public async Task ServerAction([Remainder] string stringcommand)
        {

            await _shield.Protect(Context, async () =>
            {

                var textChannel = Context.Channel;

                Command? command = await ParseStringCommand(stringcommand, textChannel);

                if (command.HasValue)
                {
                    ResCommand responce = await SendCommand(command.Value);
                    await textChannel.SendMessageAsync($"Server {command.Value.Server.Name} is {responce.action.status}");
                }
            });

        }


        public async Task<Command?> ParseStringCommand(string command, ISocketMessageChannel textChannel)
        {



            var StringCommand = new List<string>();
            StringCommand.AddRange(command.Trim().Split(' ', System.StringSplitOptions.RemoveEmptyEntries));


            var ServerName = StringCommand[0];
            var AvailableServers = Conf.ServerManager.AvailableServers;
            var Server = AvailableServers.Find(x => x.Name == ServerName);
            if (Server == null)
            {
                await textChannel.SendMessageAsync("Такого сервера нема");
                throw new ServerIsNotExist();
            }
            else
            {
                var CommandValue = StringCommand[1];




                switch ((TypeCommand) Enum.Parse(typeof(TypeCommand), CommandValue))
                {
                    case TypeCommand.start:
                        return new Command(Server, TypeCommand.start.ToString());
                      
                    case TypeCommand.reboot:
                        return new Command(Server, TypeCommand.reboot.ToString());
                        
                    default:
                        await textChannel.SendMessageAsync($"Пшёл в жопу, не такой команды для сервера {Server.Name}");
                        throw new TheCommandForTheServerIsNotExit();
                     
                }

            }
        }


        private async Task<ResCommand> SendCommand(Command command)
        {
            var client = new RestClient(baseUrl);
            var request = new RestRequest($"{command.Server.ID}/actions", DataFormat.Json).AddHeader("Authorization",
            $"Bearer {command.Server.APIKeyServer}"
            );
            request.AddJsonBody(command.commandtype);
            return await client.PostAsync<ResCommand>(request);
        }



        [Serializable]
        private class ResCommand
        {
            private CommandAction action;
            public ResCommand()
            {
                action = new CommandAction();
            }

            [Serializable]
            private class CommandAction
            {

                public CommandAction()
                {

                }

                public DateTime ?completed_at;
                public int ?id;
                public string ?region_slug;
                public int ?region_id;
                public string ?resource_type;
                public DateTime ?started_at;
                public string ?status;
                public string ?type;
            }
        }








        [Serializable]
        public struct Command
        {
            public Server Server;
            public ServerCommandType commandtype;

            public Command(Server server, string type)
            {
                Server = server;
                this.commandtype = new ServerCommandType(type);
            }
        }

        [Serializable]
        public struct ServerCommandType
        {
            public string type;

            public ServerCommandType(string type)
            {
                this.type = type;
            }
        }







    }
}