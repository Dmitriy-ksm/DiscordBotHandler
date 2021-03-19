using Discord.Commands;
using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotHandler.Function.Modules.CommandManager
{
    [Name("CommandManager")]
    public class CommandManager : ModuleBase<SocketCommandContext>    
    {
        private readonly IVerificateCommand _verifactor;
        public CommandManager(IVerificateCommand verifactor)
        {
            _verifactor = verifactor;
        }

        [Command("set")]
        [Summary("Set access to a specific command for a channel")]
        [RequireBotModerationRole]
        public Task SetCommand(string command)
        {
            _verifactor.SetPermit(command, Context.Guild.Id, Context.Channel.Id);
            return Task.CompletedTask;
        }

        [Command("unset")]
        [Summary("Unset access to a specific command for a channel")]
        [RequireBotModerationRole]
        public Task UnsetCommand(string command)
        {
            _verifactor.UnsetPermit(command, Context.Guild.Id, Context.Channel.Id);
            return Task.CompletedTask;
        }
    }
}
