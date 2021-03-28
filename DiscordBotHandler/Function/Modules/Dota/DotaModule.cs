using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Entity.Entities;
using DiscordBotHandler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotHandler.Function.Modules.Dota
{
    [Name("Dota")]
    public class DotaModule : ModuleBase<SocketCommandContext>
    {
        private readonly IDotaAssistans _dota;
        private readonly EFContext _db;
        private readonly IVerificateCommand _verificator;
        private readonly IDraw _draw;
        private readonly ILogger _logger;

        public DotaModule(IServiceProvider services/*IDotaAssistans dota, EFContext db, IVerificateCommand verificator, ILogger logger*/ )
        {
            _dota = services.GetService<IDotaAssistans>();
            _db = services.GetRequiredService<EFContext>();
            _verificator = services.GetRequiredService<IVerificateCommand>();
            _logger = services.GetRequiredService<ILogger>();
            _draw = services.GetRequiredService<IDraw>();
        }
        [Command("gameById")]
        [Summary("Getting game history")]
        public async Task GetDotaInfoes([Summary("MatchId for which you need to pull the data now")] ulong matchId)
        {
            //string debugString;
            if (_verificator.IsValid("dota", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {

                var res = await _dota.GetDotaByMatchIdAsync(matchId);
                using (MemoryStream imageStream = new MemoryStream())
                {
                    _draw.DrawImage(res, imageStream);
                    imageStream.Position = 0;
                    await Context.Channel.SendFileAsync(imageStream, "Test.jpeg");
                }
            }
            else
            {
                await _logger.LogMessage(debugString);
            }
        }
        [Command("game")]
        [Summary("Getting game history")]
        public async Task GetDotaInfoes([Summary("The (optional) user to get info from, else receives data from the sender of the message")] SocketUser user = null)
        {
            //string debugString;
            if (_verificator.IsValid("dota", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {
                var userInfo = user ?? Context.User;
                UserInfo userInfoDb = _db.UserInfos.FirstOrDefault(u => u.Id == userInfo.Id);
                if (userInfoDb != null && userInfoDb.SteamId.HasValue)
                {
                    var res = await _dota.GetDotaAsync(userInfoDb.SteamId.Value);
                    using (MemoryStream imageStream = new MemoryStream())
                    {
                        _draw.DrawImage(res, imageStream);
                        imageStream.Position = 0;
                        await Context.Channel.SendFileAsync(imageStream, "Test.jpeg");
                    }
                }
                else
                {
                    await _logger.LogMessage("Нет подходящего steamId");
                }
            }
            else
            {
                await _logger.LogMessage(debugString);
            }
        }
    }
}
