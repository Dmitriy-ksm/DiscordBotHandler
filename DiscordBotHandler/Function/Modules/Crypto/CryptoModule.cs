using Discord.Commands;
using Discord.WebSocket;
using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotHandler.Function.Modules.Crypto
{
    [Name("crypto")]
    public class CryptoModule : ModuleBase<SocketCommandContext>
    {
        private readonly EFContext _db;
        private readonly IVerificateCommand _verificator;
        private readonly ILogger _logger;
        private readonly ICrypto _cryptoService;
        public CryptoModule(IServiceProvider services )
        {
            _db = services.GetRequiredService<EFContext>();
            _verificator = services.GetRequiredService<IVerificateCommand>();
            _logger = services.GetRequiredService<ILogger>();
            _cryptoService = services.GetRequiredService<ICrypto>();
        }
        [Command("Криптовалютчик")]
        [Summary("Get crypto infoes")]
        public Task GetCrypto()
        {
            if (_verificator.IsValid("crypto", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {
                ReplyAsync(Task.Run(async () => { return await _cryptoService.GetCryptoInfoAsync(); }).Result);
            }
            else
            {
                _logger.LogMessage(debugString);
            }
            return Task.CompletedTask;
        }
    }
}
