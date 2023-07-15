using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DiscordBotHandler.Interfaces;
using Discord.Net;
using Newtonsoft.Json;
using System.Configuration;
using System.Collections.Generic;
using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Helpers.Dota;
using DiscordBotHandler.Helpers;

namespace DiscordBotHandler.Slash
{
    class SlashCommands {
        private readonly DiscordSocketClient _client;
        private readonly ILogger _log;
        private readonly IServiceProvider _services;
        private readonly IValidator _validator;
        private readonly ICrypto _cryptoService;
        private readonly IDotaAssistans _dota;
        private readonly EFContext _db;
        private readonly IDraw<DotaGameResult> _draw;

        public SlashCommands(IServiceProvider services)
        {
            _services = services;
            _client = services.GetRequiredService<DiscordSocketClient>();
            _log = services.GetRequiredService<ILogger>();
            _cryptoService = services.GetRequiredService<ICrypto>();
            _validator = services.GetRequiredService<IValidator>();
            _dota = services.GetService<IDotaAssistans>();
            _db = services.GetRequiredService<EFContext>();
            _validator = services.GetRequiredService<IValidator>();
            _draw = services.GetRequiredService<IDraw<DotaGameResult>>();

        }

        public Task InitializeAsync()
        {
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;
            return Task.CompletedTask;
        }

        List<ApplicationCommandProperties> applicationCommandProperties = new();
        private string cryptoCommandName = "криптовалютчик";
        private async Task Client_Ready() 
        {
            // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
            var guild = _client.GetGuild(Convert.ToUInt64(ConfigurationManager.AppSettings["GuildId"]));

            // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
            var cryptoCommand = new SlashCommandBuilder();

            // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
            cryptoCommand.WithName(cryptoCommandName);

            // Descriptions can have a max length of 100.
            cryptoCommand.WithDescription("Get crypto infoes");
            
            applicationCommandProperties.Add(cryptoCommand.Build());

            var gameBySteamUrlCommand = new SlashCommandBuilder()
                .WithName("game-by-steam-url")
                .WithDescription("Getting user`s game by steamURL");

            applicationCommandProperties.Add(gameBySteamUrlCommand.Build());

            var gameByIdCommand = new SlashCommandBuilder()
                .WithName("game-by-id")
                .WithDescription("Getting game by id");

            applicationCommandProperties.Add(gameByIdCommand.Build());

            var gameCommand = new SlashCommandBuilder()
                .WithName("game")
                .WithDescription("Getting discord user game");

            applicationCommandProperties.Add(gameCommand.Build());

            var addInfoesCommand = new SlashCommandBuilder()
                .WithName("add-infoes")
                .WithDescription("Adding additional info about users");

            applicationCommandProperties.Add(addInfoesCommand.Build());

            var addSteamUserCommand = new SlashCommandBuilder()
                .WithName("add-steam-user")
                .WithDescription("Adding steamId to discord user");

            applicationCommandProperties.Add(addSteamUserCommand.Build());

            try
            {
                // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
                //await guild.CreateApplicationCommandAsync(cryptoCommand.Build());
                await guild.BulkOverwriteApplicationCommandAsync(applicationCommandProperties.ToArray());
                // With global commands we don't need the guild.
                // TODO: develop only guild commands 
                // await _client.CreateGlobalApplicationCommandAsync(command.Build());
                // await _client.BulkOverwriteApplicationCommandAsync(applicationCommandProperties.ToArray());
                // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
                // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
            }
            catch(HttpException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                await _log.LogMessage(json);
            }
        }

        private async Task<Task> SlashCommandHandler(SocketSlashCommand command)
        {
            if (command.Data.Name == cryptoCommandName) 
            {
                if(!_validator.IsValid(Consts.CommandModuleNameCrypto.ToLower(), command.GuildId ?? 0, command.ChannelId ?? 0, _log))
                {
                    await command.RespondAsync($"Crypto command not allowed here");
                    return Task.CompletedTask;
                }
                await command.RespondAsync(Task.Run(async () => { return await _cryptoService.GetCryptoInfoAsync(); }).Result);
                return Task.CompletedTask;
            }
            await command.RespondAsync($"You executed {command.Data.Name}");
            return Task.CompletedTask;
        }
    }
}
