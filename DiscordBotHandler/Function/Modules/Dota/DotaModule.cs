using Discord.Commands;
using Discord.WebSocket;
using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Helpers;
using DiscordBotHandler.Helpers.Dota;
using DiscordBotHandler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DiscordBotHandler.Function.Modules.Dota
{
    [Name(Consts.CommandModuleNameDota)]
    public class DotaModule : ModuleBase<SocketCommandContext>
    {
        private readonly IDotaAssistans _dota;
        private readonly EFContext _db;
        private readonly IValidator _validator;
        private readonly IDraw<DotaGameResult> _draw;
        private readonly ILogger _logger;

        public DotaModule(IServiceProvider services)
        {
            _dota = services.GetService<IDotaAssistans>();
            _db = services.GetRequiredService<EFContext>();
            _validator = services.GetRequiredService<IValidator>();
            _logger = services.GetRequiredService<ILogger>();
            _draw = services.GetRequiredService<IDraw<DotaGameResult>>();
        }
        private bool IsValidChannel(ulong guildId, ulong channelId) => _validator.IsValid(Consts.CommandModuleNameDota.ToLower(), guildId, channelId, _logger);

        [Command("gameBySteamUrl")]
        [Summary("Getting game history")]
        public async Task GetDotaInfoes([Summary("Steam URL of the user whose game to get")] string url)
        {
            ulong steamId = Task.Run(async () => { return await _dota.GetSteamIdAsync(url); }).Result;

            if (IsValidChannel(Context.Guild.Id, Context.Channel.Id))
            {
                await HelpFunctions.GameByUrl(_dota, _draw, _logger, steamId, async (file,fileName)=>await Context.Channel.SendFileAsync(file, fileName));
            }
        }

        [Command("gameById")]
        [Summary("Getting game history")]
        public async Task GetDotaInfoes([Summary("MatchId for which you need to pull the data now")] ulong matchId)
        {
            if (IsValidChannel(Context.Guild.Id, Context.Channel.Id))
            {
                await HelpFunctions.GameById(_dota, _draw, matchId, async (file,fileName)=>await Context.Channel.SendFileAsync(file, fileName));
            }
        }

        [Command("game")]
        [Summary("Getting game history")]
        public async Task GetDotaInfoes([Summary("The (optional) user to get info from, else receives data from the sender of the message")] SocketUser user = null)
        {
            if (IsValidChannel(Context.Guild.Id, Context.Channel.Id))
            {
                ulong userInfoId = user?.Id ?? Context.User?.Id ?? 0;
                await HelpFunctions.GameByUser(_db, _dota, _draw, _logger, userInfoId, async (file,fileName)=>await Context.Channel.SendFileAsync(file, fileName));
            }
        }

        
    }
}
