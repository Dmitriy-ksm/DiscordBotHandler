using DiscordBotHandler.Interfaces;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DiscordBotHandler.Services
{
    class HeroImageProvider : IStorageProvider<Image>
    {
        public Image GetObject(params object[] obj)
        {
            if (obj.Length < 1)
            {
                return null;
            }
            else
            {

                string heroName = obj[0].ToString();
                var heroImageUrl = @"http://cdn.dota2.com/apps/dota2/images/heroes/" + heroName.Replace("npc_dota_hero_", "") + "_full.png";
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
