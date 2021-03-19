using Discord.Commands;
using DiscordBotHandler.Interfaces;
using DiscordBotHandler.Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotHandler.Function.Modules.Test
{
    [Name("Test")]
    public class TestModule: ModuleBase<SocketCommandContext>
    {
        private readonly ILogger _logger;

        public TestModule(ILogger logger)
        {
            _logger = logger;
        }
        [Command("ping")]
        [Summary("Testing Bot")]
        [RequireBotModerationRole]
        public Task PingPong()
        {
            _logger.LogMessage("PingPong");
            return ReplyAsync("Pong!");
            //if(handler.TextChannels["test"]==)
        }

    }
}
