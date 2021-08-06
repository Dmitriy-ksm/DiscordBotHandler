using Steam.Models.DOTA2;
using System;
using System.Collections.Generic;
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
    public class DotaGameResult<TDotaGameResult> : DotaGameResult { }
    public class DotaGameResult
    {
        public ulong? PlayerId;
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
    }
    public class HeroesPick
    {
        public bool IsPick { get; set; }
        public uint HeroId { get; set; }
        public string HeroName { get; set; }
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
        public uint Slot { get; set; }
    }
}
