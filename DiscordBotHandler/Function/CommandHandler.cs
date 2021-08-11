using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
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
        private readonly ICooldown _cooldown;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            _services = services;
            _client = services.GetRequiredService<DiscordSocketClient>();
            _commands = services.GetRequiredService<CommandService>();
            _log = services.GetRequiredService<ILogger>();
            _wordSearch = services.GetRequiredService<IWordSearch>();
            _cooldown = services.GetRequiredService<ICooldown>();
        }
        public Task InitializeAsync()
        {
            _commands.CommandExecuted += CommandExecutedAsync;
            _client.MessageReceived += MessageReceived;
            _client.Log += Log;
            return Task.CompletedTask;
        }

        public Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
                return Task.CompletedTask;

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
            {
                _ = _log.LogMessage($"{command.Value.Name} successed");
                return Task.CompletedTask;
            }

            // the command failed, let's notify the user that something happened.
            _ = _log.LogMessage($"error: {result}");
            return Task.CompletedTask;
        }

        public async Task MessageReceived(SocketMessage msg)
        {
            if (msg.Author.Id == _client.CurrentUser.Id)
                return;

            var message = msg as SocketUserMessage;
            if (message == null || message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, message);
            string reply = _cooldown.Check("wordsearch") ? _wordSearch.SearchWord(context.Guild.Id, msg.Content) : null;
           
            if (reply != null)
            {
                _cooldown.Set("wordsearch");
                await message.Channel.SendMessageAsync(reply);
            }
            int argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos) ||
                    message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                return;

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
