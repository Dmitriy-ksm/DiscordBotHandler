using DiscordBotHandler.Helpers.Dota;
using DiscordBotHandler.Interfaces;
using Steam.Models.DOTA2;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
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
            var response = await dotaInterface.GetMatchHistoryAsync(accountId: accountId, matchesRequested: "1");
            if (response.Data.Matches != null)
            {
                var Match = response.Data.Matches.First();
                return await GetDota(Match.MatchId);
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
        public async Task<DotaGameResult> GetDota(ulong matchId)
        {
            var responseFull = await dotaInterface.GetMatchDetailsAsync(matchId);
            var MatchDetails = responseFull.Data;
            var Players = MatchDetails.Players.AsEnumerable();
            DotaGameResult returnValue = new DotaGameResult()
            {
                MatchId = matchId,
                RadiantWin = MatchDetails.RadiantWin,
                Duration = MatchDetails.Duration,
                BarracksStatesDire = MatchDetails.BarracksStatesDire,
                BarracksStatesRadiant = MatchDetails.BarracksStatesRadiant,
                TowerStatesDire = MatchDetails.TowerStatesDire,
                TowerStatesRadiant = MatchDetails.TowerStatesRadiant,
                PicksAndBans = new List<HeroesPick>(),
                Players = new List<DotaPlayer>(),
                StartTime = MatchDetails.StartTime
            };
            foreach(var pb in MatchDetails.PicksAndBans)
            {
                var hero = Heroes.FirstOrDefault((h) => h.Id == pb.HeroId);
                returnValue.PicksAndBans.Add(new HeroesPick()
                {
                    IsPick = pb.IsPick,
                    Order = pb.Order,
                    Team = pb.Team,
                    HeroId = hero.Id,
                    HeroName = hero.LocalizedName
                });
            }
            foreach (var player in Players)
            {
                var dotaPlayer = DotaPlayerExtension.DefaultInitialize(player);
                dotaPlayer.HeroName = Heroes.FirstOrDefault((h) => h.Id == player.HeroId)?.LocalizedName;
                dotaPlayer.SetPlayerItems(player, Items);
                returnValue.Players.Add(dotaPlayer);
            }
            return returnValue;
        }
    }
}
