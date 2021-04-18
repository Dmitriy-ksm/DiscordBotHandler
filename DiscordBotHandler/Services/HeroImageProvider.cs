using DiscordBotHandler.Interfaces;
using SixLabors.ImageSharp;
using System.Net;

namespace DiscordBotHandler.Services
{
    class HeroImageProvider : IStorageProvider<Image>
    {
        private static bool useCloudflare = true;
        public Image GetObject(params object[] obj)
        {
            if (obj.Length < 1)
            {
                return null;
            }
            else
            {

                string heroName = obj[0].ToString();
                var heroImageUrl = useCloudflare ?  @"https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/" + heroName.Replace("npc_dota_hero_", "") + ".png" : @"http://cdn.dota2.com/apps/dota2/images/heroes/" + heroName.Replace("npc_dota_hero_", "") + "_full.png";
                var requestItem = WebRequest.Create(heroImageUrl);

                using (var responseItem = requestItem.GetResponse())
                using (var streamItem = responseItem.GetResponseStream())
                {
                    return Image.Load(streamItem);
                }
            }
        }
    }
}
