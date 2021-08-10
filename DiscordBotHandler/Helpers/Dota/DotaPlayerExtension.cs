using Steam.Models.DOTA2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBotHandler.Helpers.Dota
{
    public static class DotaPlayerExtension
    {
        public static DotaPlayer DefaultInitialize(MatchPlayerModel data)
        {
            return new DotaPlayer()
            {
                AccountId = data.AccountId,
                HeroId = data.HeroId,
                Level = data.Level,
                Kills = data.Kills,
                Deaths = data.Deaths,
                Assists = data.Assists,
                LastHits = data.LastHits,
                Denies = data.Denies,
                GoldPerMinute = data.GoldPerMinute,
                ExperiencePerMinute = data.ExperiencePerMinute,
                NetWorth = data.GoldSpent,
                HeroDamage = data.HeroDamage,
                HeroHealing = data.HeroHealing,
                TowerDamage = data.TowerDamage
            };
        }
        public static void SetPlayerItems(this DotaPlayer obj, MatchPlayerModel data, List<GameItem> allItems)
        {
            obj.Items = new List<DotaItems>() {
                        AddItemToSlot(data.Item0,0,allItems),
                        AddItemToSlot(data.Item1,1,allItems),
                        AddItemToSlot(data.Item2,2,allItems),
                        AddItemToSlot(data.Item3,3,allItems),
                        AddItemToSlot(data.Item4,4,allItems),
                        AddItemToSlot(data.Item5,5,allItems),
                        AddItemToSlot(data.ItemNeutral,6,allItems)
                    };
            obj.BackPacks = new List<DotaItems>()
                    {
                        AddItemToSlot(data.Backpack0,0,allItems),
                        AddItemToSlot(data.Backpack1,1,allItems),
                        AddItemToSlot(data.Backpack2,2,allItems)
                    };
        }

        private static DotaItems AddItemToSlot(uint item, int slot, List<GameItem> allItems)
        {
            return item != 0 ? new DotaItems()
            {
                ItemId = item,
                Slot = slot,
                ItemName = allItems.FirstOrDefault(i => i.Id == item).LocalizedName
            } : new DotaItems();
        }
    }
}
