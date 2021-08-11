using Discord.Commands;
using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DiscordBotHandler.Function.Modules.Crypto
{
    [Name("crypto")]
    public class CryptoModule : ModuleBase<SocketCommandContext>
    {
        private readonly IValidator _validator;
        private readonly ILogger _logger;
        private readonly ICrypto _cryptoService;
        public CryptoModule(IServiceProvider services )
        {
            _validator = services.GetRequiredService<IValidator>();
            _logger = services.GetRequiredService<ILogger>();
            _cryptoService = services.GetRequiredService<ICrypto>();
        }
        private bool IsValidChannel(ulong guildId, ulong channelId) => _validator.IsValid("crypto", guildId, channelId, _logger);

        [Command("Криптовалютчик")]
        [Summary("Get crypto infoes")]
        public Task GetCrypto()
        {
            if (IsValidChannel(Context.Guild.Id, Context.Channel.Id))
                ReplyAsync(Task.Run(async () => { return await _cryptoService.GetCryptoInfoAsync(); }).Result);

            return Task.CompletedTask;
        }
    }
}
