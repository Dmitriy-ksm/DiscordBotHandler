using Steam.Models.DOTA2;
using System;
using System.Collections.Generic;

namespace DiscordBotHandler.Helpers.Dota
{
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
        //TODO loneDruid 
        /*
          "additional_units": [
                    {
                        "unitname": "spirit_bear",
                        "item_0": 50,
                        "item_1": 168,
                        "item_2": 11,
                        "item_3": 135,
                        "item_4": 55,
                        "item_5": 172,
                        "backpack_0": 0,
                        "backpack_1": 0,
                        "backpack_2": 0,
                        "item_neutral": 0
                    }
                ]
         */
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
        public List<DotaItems> BackPacks { get; set; }
    }

    public class DotaItems
    {
        public uint ItemId { get; set; }
        public string ItemName { get; set; }
        public int Slot { get; set; }
    }
}
