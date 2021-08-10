using DiscordBotHandler.Helpers.Dota;
using Steam.Models.DOTA2;
using System.Threading.Tasks;

namespace DiscordBotHandler.Interfaces
{
    public interface IDotaAssistans
    {
        public Hero GetHeroById(uint id);
        public GameItem GetItemById(uint id);
        public Task<ulong> GetSteamIdAsync(string url);
        public Task<DotaGameResult> GetDotaAsync(ulong accountId);
        public Task<DotaGameResult> GetDotaByMatchIdAsync(ulong matchId);
    }
   
}
