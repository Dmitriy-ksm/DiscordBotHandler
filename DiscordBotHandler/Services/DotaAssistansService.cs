using DiscordBotHandler.Helpers.Dota;
using DiscordBotHandler.Interfaces;
using Newtonsoft.Json;
using Steam.Models.DOTA2;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordBotHandler.Services
{

    public class DotaAssistansService : IDotaAssistans
    {
        private SteamWebInterfaceFactory webInterfaceFactory;
        private DOTA2Econ dotaEconInterface;
        private DOTA2Match _dotaInterface;
        private SteamUser _steamInterface;

        SteamUser SteamInterface => _steamInterface ??= webInterfaceFactory.CreateSteamWebInterface<SteamUser>(new HttpClient());
        DOTA2Match DotaInterface => _dotaInterface ??= webInterfaceFactory.CreateSteamWebInterface<DOTA2Match>(new HttpClient());
        
        private List<Hero> Heroes;
        private List<GameItem> Items;
        public DotaAssistansService()
        {
            webInterfaceFactory = new SteamWebInterfaceFactory(ConfigurationManager.AppSettings["steamWebApi"]);
            dotaEconInterface = webInterfaceFactory.CreateSteamWebInterface<DOTA2Econ>(new HttpClient());
            GetHeroes();
            GetItems();
        }
         private Hero defaultHeroes = new Hero {Id = uint.MaxValue, Name = "unknowm"};
        private GameItem defaultGameItem = new GameItem {Id = uint.MaxValue, Name = "unknowm"};
        public Hero GetHeroById(uint id) => Heroes.FirstOrDefault(h => h.Id == id, defaultHeroes);
        public GameItem GetItemById(uint id) => Items.FirstOrDefault(i => i.Id == id, defaultGameItem);

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
                    result = (await SteamInterface.ResolveVanityUrlAsync(vanityToResolve, 1)).Data;
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

        public async Task<DotaGameResult> GetDotaByMatchIdAsync(ulong matchId) => await GetDota(matchId);

        public async Task<DotaGameResult> GetDotaAsync(ulong accountId)
        {
            var matchId =  await GetLastMatchBySteamId(accountId);
            if(matchId > 0)
            {
                return await GetDota(matchId);
            }
            return null;
        }

        public async Task<ulong> GetLastMatchBySteamId(ulong accountId)
        {
            var response = await DotaInterface.GetMatchHistoryAsync(accountId: accountId, matchesRequested: "1");
            if (response.Data.Matches != null)
            {
                var Match = response.Data.Matches.First();
                return Match.MatchId;
            }
            else
                return 0;
        }

        async void GetHeroes() => Heroes = (await dotaEconInterface.GetHeroesAsync()).Data.ToList();
        //async void GetItems() => Items = (await dotaEconInterface.GetGameItemsAsync()).Data.ToList();
        void GetItems()
        {
            Items = new List<GameItem>();
            using (StreamReader r = new StreamReader("item.json"))
            {
                string json = r.ReadToEnd();
                dynamic array = JsonConvert.DeserializeObject(json);
                foreach (var item in array.items)
                {
                    GameItem gameItem = new GameItem();
                    gameItem.Id = item.id;
                    gameItem.Name = item.name;
                    Items.Add(gameItem);
                }
            }
        }

        public async Task<DotaGameResult> GetDota(ulong matchId)
        {
            var responseFull = await DotaInterface.GetMatchDetailsAsync(matchId);
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
