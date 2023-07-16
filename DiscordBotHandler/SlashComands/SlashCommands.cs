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
using System.Linq;

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
        private const string cryptoCommandName = "криптовалютчик";
        private const string gameFirstCommandName = "game-by-steam-url";
        private const string gameSecondCommandName = "game-by-id";
        private const string gameThirdCommandName = "game";
        private const string addInfoCommandName = "add-infoes";
        private const string addSteamUserCommandName = "add-steam-user";
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

            var gameCommand = new SlashCommandBuilder()
                .WithName("dota")
                .WithDescription("Commands related to dota")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName(gameFirstCommandName)
                    .WithDescription("Getting user`s game by steamURL")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("url", ApplicationCommandOptionType.String, "Steam user URL", isRequired: true))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName(gameSecondCommandName)
                    .WithDescription("Getting game by id")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("id", ApplicationCommandOptionType.Integer, "Game ID", isRequired: true))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName(gameThirdCommandName)
                    .WithDescription("Getting discord user game")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("user", ApplicationCommandOptionType.User, "User, whos game need to be load"));

            applicationCommandProperties.Add(gameCommand.Build());

            var userCommand = new SlashCommandBuilder()
                .WithName("user")
                .WithDescription("Commands related to user management")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName(addInfoCommandName)
                    .WithDescription("Adding additional info about users")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("key", ApplicationCommandOptionType.String, "DB key", isRequired:true)
                    .AddOption("info", ApplicationCommandOptionType.String, "Info"))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName(addSteamUserCommandName)
                    .WithDescription("Adding steamId to discord user")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("url", ApplicationCommandOptionType.String, "Steam user URL", isRequired: true));

            //applicationCommandProperties.Add(userCommand.Build());

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

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            string subCommandName = command.Data?.Options?.First()?.Name ?? "";
            ulong guildId = command.GuildId ?? 0;
            ulong channelId = command.ChannelId ?? 0;
            switch(command.Data.Name)
            {
                case cryptoCommandName:
                    if(_validator.IsValid(Consts.CommandModuleNameCrypto.ToLower(), guildId, channelId, _log))
                    {
                        await command.RespondAsync(Task.Run(async () => { return await _cryptoService.GetCryptoInfoAsync(); }).Result);
                    }
                    break;
                case "dota":
                    if (_validator.IsValid(Consts.CommandModuleNameDota.ToLower(), guildId, channelId, _log))
                    {
                        switch(subCommandName) 
                        {
                            case gameFirstCommandName:
                                string url = (string)(command.Data.Options.First().Options?.First()?.Value ?? "");
                                if (url == "") break;
                                ulong steamId = Task.Run(async () => { return await _dota.GetSteamIdAsync(url); }).Result;
                                await HelpFunctions.GameByUrl(_dota, _draw, _log, steamId, async (file,fileName)=>await command.RespondWithFileAsync(file, fileName));
                                break;
                            case gameSecondCommandName:
                                ulong matchId = (ulong)(command.Data.Options.First().Options?.First()?.Value ?? 0);
                                if(matchId == 0) break;
                                await HelpFunctions.GameById(_dota, _draw, matchId, async (file,fileName)=>await command.RespondWithFileAsync(file, fileName));
                                break;
                            case gameThirdCommandName:
                                ulong userInfoId = ((SocketUser)command.Data.Options.First().Options?.FirstOrDefault()?.Value)?.Id ?? command.User?.Id ?? 0;
                                await HelpFunctions.GameByUser(_db, _dota, _draw, _log, userInfoId, async (file,fileName)=>await command.RespondWithFileAsync(file, fileName));
                                break;
                        }
                    }
                    break;
                case "user":
                    switch(subCommandName) 
                    {
                        case addInfoCommandName:
                            break;
                        case addSteamUserCommandName:
                            break;
                    }
                    break;
            }
            _log.LogMessage($"You executed {command.Data.Name}");
        }
    }
}
