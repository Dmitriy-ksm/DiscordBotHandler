using Steam.Models.DOTA2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotHandler.Interfaces
{
    public interface IDotaAssistans
    {
        public Task<ulong> GetSteamIdAsync(string url);
        public Task<DotaGameResult> GetDotaAsync(ulong accountId);
        public Task<DotaGameResult> GetDotaByMatchIdAsync(ulong matchId);
    }
    public class DotaGameResult
    {
        public ulong MatchId;
        public DateTime StartTime;
        public bool RadiantWin;
        public uint Duration;
        public TowerState TowerStatesDire;
        public BarracksState BarracksStatesRadiant;
        public BarracksState BarracksStatesDire;
        public TowerState TowerStatesRadiant;
        public List<HeroesPick> PicksAndBans;
        public List<DotaPlayer> Players;
        public override string ToString()
        {
            var ret_val = "ID: " + MatchId + (RadiantWin ? " Radiant win " : " Dire win ") + TimeSpan.FromSeconds(Duration).ToString() + Environment.NewLine + " Heroes:";
            foreach (var player in Players)
            {
                ret_val += Environment.NewLine + player.HeroName;

                ret_val += Environment.NewLine + "   Уровень:" + player.Level;
                ret_val += Environment.NewLine + "   Убийст:" + player.Kills;
                ret_val += Environment.NewLine + "   Смертей:" + player.Deaths;
                ret_val += Environment.NewLine + "   Ассистов:" + player.Assists;
                ret_val += Environment.NewLine + "   Исцеление:" + player.HeroHealing;
                ret_val += Environment.NewLine + "   Урон:" + player.HeroDamage;
                ret_val += Environment.NewLine + "   Урон по строениям:" + player.TowerDamage;
                ret_val += Environment.NewLine + "   Шмотки:";
                foreach (var item in player.Items)
                {
                    ret_val += item.ItemId != 0 ? item.ItemName + "," : "";
                }
            }
            ret_val += Environment.NewLine + " Game Time:" + StartTime.ToString();
            return ret_val;
        }
    }
    public class HeroesPick
    {
        public bool IsPick { get; set; }
        public uint HeroId { get; set; }
        public string HeroName { get; set; }
        public string HeroImageUrl { get; set; }
        public uint Team { get; set; }
        public uint Order { get; set; }
    }
    public class DotaPlayer
    {
        public uint AccountId { get; set; }
        public uint Level { get; set; }
        public uint Kills { get; set; }
        public uint Deaths { get; set; }
        public uint Assists { get; set; }
        public uint HeroDamage { get; set; }
        public uint TowerDamage { get; set; }
        public uint HeroHealing { get; set; }
        public uint HeroId { get; set; }
        public string HeroName { get; set; }
        public string HeroImageUrl { get; set; }
        public uint Denies { get; set; }
        public uint LastHits { get; set; }
        public uint NetWorth { get; set; }
        public uint ExperiencePerMinute { get; set; }
        public uint GoldPerMinute { get; set; }
        public List<DotaItems> Items { get; set; }
    }
    public class DotaItems
    {
        public uint ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemImageUrl { get; set; }
        public uint Slot { get; set; }
    }
}
