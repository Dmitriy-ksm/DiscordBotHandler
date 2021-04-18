extern alias SteamWebAPI2Custom;
extern alias SteamModelsCustom;
using DiscordBotHandler.Interfaces;
using SteamModelsCustom::Steam.Models.DOTA2;
using SteamWebAPI2Custom::SteamWebAPI2.Interfaces;
using SteamWebAPI2Custom::SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotHandler.Services
{
    class DotaAssistans : IDotaAssistans
    {
        private SteamWebInterfaceFactory webInterfaceFactory;
        private DOTA2Econ dotaEconInterface;
        private DOTA2Match dotaInterface;
        private SteamUser steamInterface;
        private List<Hero> Heroes;
        private List<GameItem> Items;
        public DotaAssistans()
        {
            webInterfaceFactory = new SteamWebInterfaceFactory(ConfigurationManager.AppSettings["steamWebApi"]);
            dotaEconInterface = webInterfaceFactory.CreateSteamWebInterface<DOTA2Econ>(new HttpClient());
            steamInterface = webInterfaceFactory.CreateSteamWebInterface<SteamUser>(new HttpClient());
            dotaInterface = webInterfaceFactory.CreateSteamWebInterface<DOTA2Match>(new HttpClient());
            GetHeroes();
            GetItems();
        }
        public Hero GetHeroById(uint id)
        {
            return Heroes.FirstOrDefault(h => h.Id == id);
        }
        public GameItem GetItemById(uint id)
        {
            return Items.FirstOrDefault(i => i.Id == id);
        }
        public async Task<ulong> GetSteamIdAsync(string url)
        {
            ulong result = 0;
            var vanityToResolve = "";
            try
            {
                if (url.Contains("/id/"))
                {
                    string[] urlElements = url.Split("/", StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < urlElements.Length; i++)
                    {
                        if (urlElements[i] == "id" && i + 1 < urlElements.Length)
                        {
                            vanityToResolve = urlElements[i + 1];
                            break;
                        }
                    }
                    result = (await steamInterface.ResolveVanityUrlAsync(vanityToResolve, 1)).Data;
                }
                else if (url.Contains("/profiles/"))
                {
                    string[] urlElements = url.Split("/", StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < urlElements.Length; i++)
                    {
                        if (urlElements[i] == "profiles" && i + 1 < urlElements.Length)
                        {
                            result = Convert.ToUInt64(urlElements[i + 1]);
                            break;
                        }
                    }
                }
            }
            catch
            {
                result = 0;
            }
            return result;
        }
        public async Task<DotaGameResult> GetDotaByMatchIdAsync(ulong matchId)
        {
            return await GetDota(matchId);
        }
        public async Task<DotaGameResult> GetDotaAsync(ulong accountId)
        {
            var accountId32b = accountId - 76561197960265728;
            var response = await dotaInterface.GetMatchHistoryAsync(accountId: accountId, matchesRequested: "1");
            if (response.Data.Matches != null)
            {
                var Match = response.Data.Matches.First();
                return await GetDota(Match.MatchId, accountId);
            }
            return null;
        }
        public async void GetHeroes()
        {
            Heroes = (await dotaEconInterface.GetHeroesAsync()).Data.ToList();
        }
        public async void GetItems()
        {
            Items = (await dotaEconInterface.GetGameItemsAsync()).Data.ToList();
        }
        public async Task<DotaGameResult> GetDota(ulong matchId, ulong accountId = 0)
        {
            var responseFull = await dotaInterface.GetMatchDetailsAsync(matchId);
            var MatchDetails = responseFull.Data;
            var Players = MatchDetails.Players.AsEnumerable();
            DotaGameResult returnValue = new DotaGameResult()
            {
                PlayerId = accountId,
                MatchId = matchId,
                RadiantWin = MatchDetails.RadiantWin,
                Duration = MatchDetails.Duration,
                BarracksStatesDire = MatchDetails.BarracksStatesDire,
                BarracksStatesRadiant = MatchDetails.BarracksStatesRadiant,
                TowerStatesDire = MatchDetails.TowerStatesDire,
                TowerStatesRadiant = MatchDetails.TowerStatesRadiant,
                PicksAndBans = new List<HeroesPick>(),
                Players = new List<DotaPlayer>(),
                StartTime = MatchDetails.StartTime,
                RadiantScore = MatchDetails.RadiantScore,
                DireScore = MatchDetails.DireScore
            };
            foreach (var pb in MatchDetails.PicksAndBans)
            {
                var hero = Heroes.FirstOrDefault((h) => h.Id == pb.HeroId);
                returnValue.PicksAndBans.Add(new HeroesPick()
                {
                    IsPick = pb.IsPick,
                    Order = pb.Order,
                    Team = pb.Team,
                    HeroId = hero.Id,
                    HeroImageUrl = @"http://cdn.dota2.com/apps/dota2/images/heroes/" + hero.Name.Replace("npc_dota_hero_", "") + "_full.png",
                    HeroName = hero.LocalizedName
                });
            }
            foreach (var player in Players)
            {
                var hero = Heroes.FirstOrDefault((h) => h.Id == player.HeroId);
                returnValue.Players.Add(new DotaPlayer()
                {
                    AccountId = player.AccountId,
                    HeroId = player.HeroId,
                    HeroName = hero.LocalizedName,
                    HeroImageUrl = @"http://cdn.dota2.com/apps/dota2/images/heroes/" + hero.Name.Replace("npc_dota_hero_", "") + "_full.png",
                    Level = player.Level,
                    Kills = player.Kills,
                    Deaths = player.Deaths,
                    Assists = player.Assists,
                    LastHits = player.LastHits,
                    Denies = player.Denies,
                    GoldPerMinute = player.GoldPerMinute,
                    ExperiencePerMinute = player.ExperiencePerMinute,
                    NetWorth = player.GoldSpent,
                    HeroDamage = player.HeroDamage,
                    HeroHealing = player.HeroHealing,
                    TowerDamage = player.TowerDamage,
                    Items = new List<DotaItems>() {
                        player.Item0 != 0 ? new DotaItems(){
                            ItemId=player.Item0,
                            Slot=0,
                            ItemName =  Items.FirstOrDefault(i => i.Id == player.Item0).LocalizedName,
                            ItemImageUrl = @"http://cdn.dota2.com/apps/dota2/images/items/"+Items.FirstOrDefault(i=>i.Id==player.Item0).Name.Replace("item_", "")+"_lg.png"
                        }:new DotaItems(),
                        player.Item1 != 0 ?  new DotaItems(){
                            ItemId=player.Item1,
                            Slot=1,
                            ItemName =  Items.FirstOrDefault(i => i.Id == player.Item1).LocalizedName,
                            ItemImageUrl = @"http://cdn.dota2.com/apps/dota2/images/items/"+Items.FirstOrDefault(i=>i.Id==player.Item1).Name.Replace("item_", "")+"_lg.png"
                        }:new DotaItems(),
                         player.Item2 != 0 ?  new DotaItems(){
                            ItemId=player.Item2,
                            Slot=2,
                            ItemName =  Items.FirstOrDefault(i => i.Id == player.Item2).LocalizedName,
                            ItemImageUrl = @"http://cdn.dota2.com/apps/dota2/images/items/"+Items.FirstOrDefault(i=>i.Id==player.Item2).Name.Replace("item_", "")+"_lg.png"
                        }:new DotaItems(),
                         player.Item3 != 0 ?   new DotaItems(){
                            ItemId=player.Item3,
                            Slot=3,
                            ItemName =  Items.FirstOrDefault(i => i.Id == player.Item3).LocalizedName,
                            ItemImageUrl = @"http://cdn.dota2.com/apps/dota2/images/items/"+Items.FirstOrDefault(i=>i.Id==player.Item3).Name.Replace("item_", "")+"_lg.png"
                        }:new DotaItems(),
                         player.Item4 != 0 ?    new DotaItems(){
                            ItemId=player.Item4,
                            Slot=4,
                            ItemName =  Items.FirstOrDefault(i => i.Id == player.Item4).LocalizedName,
                            ItemImageUrl = @"http://cdn.dota2.com/apps/dota2/images/items/"+Items.FirstOrDefault(i=>i.Id==player.Item4).Name.Replace("item_", "")+"_lg.png"
                        }:new DotaItems(),
                          player.Item5 != 0 ?    new DotaItems(){
                            ItemId=player.Item5,
                            Slot=5,
                            ItemName =  Items.FirstOrDefault(i => i.Id == player.Item5).LocalizedName,
                            ItemImageUrl = @"http://cdn.dota2.com/apps/dota2/images/items/"+Items.FirstOrDefault(i=>i.Id==player.Item5).Name.Replace("item_", "")+"_lg.png"
                        }:new DotaItems(),
                          player.ItemNeutral != 0 ?    new DotaItems(){
                            ItemId=player.ItemNeutral,
                            Slot=6,
                            ItemName =  Items.FirstOrDefault(i => i.Id == player.ItemNeutral).LocalizedName,
                            ItemImageUrl = @"http://cdn.dota2.com/apps/dota2/images/items/"+Items.FirstOrDefault(i=>i.Id==player.ItemNeutral).Name.Replace("item_", "")+"_lg.png"
                        }:new DotaItems()
                              /*new DotaItems(){
                            ItemId=player.Item0,
                            Slot=0,
                            ItemName =  Items.FirstOrDefault(i => i.Id == player.Item0).LocalizedName,
                            ItemImageUrl = @"http://cdn.dota2.com/apps/dota2/images/items/"+Items.FirstOrDefault(i=>i.Id==player.Item0).Name.Replace("item_", "")+"_lg.png"
                        }*/
                    }
                });
            }
            return returnValue;
        }
    }
}
