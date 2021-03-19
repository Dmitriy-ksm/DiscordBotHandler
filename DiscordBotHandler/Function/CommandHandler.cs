using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DiscordBotHandler.Logger;
using System.Configuration;
using System.Linq;
using SteamWebAPI2.Interfaces;
using Steam.Models.DOTA2;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using DiscordBotHandler.Interfaces;

namespace DiscordBotHandler.Function
{
    class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly ILogger _log;
        private readonly IWordSearch _wordSearch;

        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            _services = services;
            _client = services.GetRequiredService<DiscordSocketClient>();
            _commands = services.GetRequiredService<CommandService>();
            _log = services.GetRequiredService<ILogger>();
            _wordSearch = services.GetRequiredService<IWordSearch>();
        }
        public async Task InitializeAsync()
        {
            
            _commands.CommandExecuted += CommandExecutedAsync;
            _client.MessageReceived += MessageReceived;
            _client.Log += Log;
            //await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
                return;

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
            {
                _log.LogMessage($"{command.Value.Name} successed");
                return;
            }

            // the command failed, let's notify the user that something happened.
            _log.LogMessage($"error: {result}");
            //await context.Channel.SendMessageAsync($"error: {result}");
        }

        public async Task MessageReceived(SocketMessage msg)
        {
            if (msg.Author.Id == _client.CurrentUser.Id)
                return;
            var message = msg as SocketUserMessage;
            if (message == null || message.Author.IsBot)
                return;
            var context = new SocketCommandContext(_client, message);
            string reply = _wordSearch.SearchWord(context.Guild.Id, msg.Content);
            if (reply != null)
            {
                message.Channel.SendMessageAsync(reply);
            }
            int argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos) ||
                    message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                return;
            //var user = message.Author;
            await _commands.ExecuteAsync(context, argPos, _services);
        }
        public async Task<Task> Log(LogMessage msg)
        {
            return await _log.LogMessage(msg.ToString());
        }
        public async Task<Task> Log(string msg)
        {
            return await _log.LogMessage(msg);
        }
    }
}
