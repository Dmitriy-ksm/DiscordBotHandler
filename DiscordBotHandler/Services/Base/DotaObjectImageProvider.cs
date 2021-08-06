using DiscordBotHandler.Interfaces;
using SixLabors.ImageSharp;
using System.Net;

namespace DiscordBotHandler.Services.Providers
{
    class DotaObjectImageProvider : IStorageProvider<Image>
    {
        private readonly StorageContains _type;
        public DotaObjectImageProvider(StorageContains type)
        {
            _type = type;
        }
        public Image GetObject(params object[] obj)
        {
            if (obj.Length < 1)
            {
                return null;
            }
            else
            {
                string url = string.Empty;

                string objName = obj[0].ToString();

                if (_type.Equals(StorageContains.DotaHero))
                    url = @"https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/" + objName.Replace("npc_dota_hero_", "") + ".png";
                if(_type.Equals(StorageContains.DotaItem))
                    url = @"https://cdn.cloudflare.steamstatic.com/apps/dota2/images//items/" + objName.Replace("item_", "") + "_lg.png";

                var requestItem = WebRequest.Create(url);
                using (var responseItem = requestItem.GetResponse())
                {
                    using (var streamItem = responseItem.GetResponseStream())
                    {
                        return Image.Load(streamItem);
                    }
                }
            }
        }
    }
}
