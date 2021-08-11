using Discord.Commands;
using Discord.WebSocket;
using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Entity.Entities;
using DiscordBotHandler.Helpers.Dota;
using DiscordBotHandler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotHandler.Function.Modules.Dota
{
    [Name("Dota")]
    public class DotaModule : ModuleBase<SocketCommandContext>
    {
        private readonly IDotaAssistans _dota;
        private readonly EFContext _db;
        private readonly IValidator _validator;
        private readonly IDraw<DotaGameResult> _draw;
        private readonly ILogger _logger;

        public DotaModule(IServiceProvider services/*IDotaAssistans dota, EFContext db, IVerificateCommand verificator, ILogger logger*/ )
        {
            _dota = services.GetService<IDotaAssistans>();
            _db = services.GetRequiredService<EFContext>();
            _validator = services.GetRequiredService<IValidator>();
            _logger = services.GetRequiredService<ILogger>();
            _draw = services.GetRequiredService<IDraw<DotaGameResult>>();
        }
        private bool IsValidChannel(ulong guildId, ulong channelId) => _validator.IsValid("dota", guildId, channelId, _logger);

        [Command("gameBySteamUrl")]
        [Summary("Getting game history")]
        public async Task GetDotaInfoes([Summary("Steam URL of the user whose game to get")] string url)
        {
            ulong steamId = Task.Run(async () => { return await _dota.GetSteamIdAsync(url); }).Result;

            if (IsValidChannel(Context.Guild.Id, Context.Channel.Id))
            {
                if (steamId > 0)
                {
                    var res = await _dota.GetDotaAsync(steamId);
                    res.PlayerId = steamId;
                    using (MemoryStream imageStream = new MemoryStream())
                    {
                        _draw.DrawImage(res, imageStream);
                        imageStream.Position = 0;
                        await Context.Channel.SendFileAsync(imageStream, "Test.jpeg");
                    }
                }
                else
                    _ = _logger.LogMessage("Нет подходящего steamId");
            }
        }
        [Command("gameById")]
        [Summary("Getting game history")]
        public async Task GetDotaInfoes([Summary("MatchId for which you need to pull the data now")] ulong matchId)
        {
            if (IsValidChannel(Context.Guild.Id, Context.Channel.Id))
            {

                var res = await _dota.GetDotaByMatchIdAsync(matchId);
                using (MemoryStream imageStream = new MemoryStream())
                {
                    _draw.DrawImage(res, imageStream);
                    imageStream.Position = 0;
                    await Context.Channel.SendFileAsync(imageStream, "Test.jpeg");
                }
            }
        }
        [Command("game")]
        [Summary("Getting game history")]
        public async Task GetDotaInfoes([Summary("The (optional) user to get info from, else receives data from the sender of the message")] SocketUser user = null)
        {
            if (IsValidChannel(Context.Guild.Id, Context.Channel.Id))
            {
                var userInfo = user ?? Context.User;
                UserInfo userInfoDb = _db.UserInfos.FirstOrDefault(u => u.Id == userInfo.Id);
                if (userInfoDb != null && userInfoDb.SteamId.HasValue)
                {
                    var res = await _dota.GetDotaAsync(userInfoDb.SteamId.Value);
                    res.PlayerId = userInfoDb.SteamId;
                    using (MemoryStream imageStream = new MemoryStream())
                    {
                        _draw.DrawImage(res, imageStream);
                        imageStream.Position = 0;
                        await Context.Channel.SendFileAsync(imageStream, "Test.jpeg");
                    }
                }
                else
                    _ = _logger.LogMessage("Нет подходящего steamId");
            }
        }
    }
}
