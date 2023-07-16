using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Entity.Entities;
using DiscordBotHandler.Helpers.Dota;
using DiscordBotHandler.Interfaces;

namespace DiscordBotHandler.Helpers
{
    public delegate Task SendImage(MemoryStream file, string fileName);

    public static class HelpFunctions {
        public static async Task GameById(IDotaAssistans _dota, IDraw<DotaGameResult> _draw, ulong matchId, SendImage sendImage)
        {
            var res = await _dota.GetDotaByMatchIdAsync(matchId);
            await SendImage(_draw, sendImage, res);
        }

        private static async Task SendImage(IDraw<DotaGameResult> _draw, SendImage sendImage, DotaGameResult res)
        {
            using (MemoryStream imageStream = new MemoryStream())
            {
                _draw.DrawImage(res, imageStream);
                imageStream.Position = 0;
                await sendImage(imageStream, "Test.jpeg");
                //await Context.Channel.SendFileAsync(imageStream, "Test.jpeg");
            }
        }

        public static async Task GameByUrl(IDotaAssistans _dota, IDraw<DotaGameResult> _draw, ILogger _logger, ulong steamId, SendImage sendImage)
        {
            if (steamId > 0)
            {
                var res = await _dota.GetDotaAsync(steamId);
                res.PlayerId = steamId;
                await SendImage(_draw, sendImage, res);
            }
            else
                _ = _logger.LogMessage("Нет подходящего steamId");
        }

        public static async Task GameByUser(EFContext _db, IDotaAssistans _dota, IDraw<DotaGameResult> _draw, ILogger _logger, ulong userInfoId, SendImage sendImage)
        {
            UserInfo userInfoDb = _db.UserInfos.FirstOrDefault(u => u.Id == userInfoId);
            if (userInfoDb != null && userInfoDb.SteamId.HasValue)
            {
                var res = await _dota.GetDotaAsync(userInfoDb.SteamId.Value);
                res.PlayerId = userInfoDb.SteamId;
                await SendImage(_draw, sendImage, res);
            }
            else
                _ = _logger.LogMessage("Нет подходящего steamId");
        }
    }
    
}
        