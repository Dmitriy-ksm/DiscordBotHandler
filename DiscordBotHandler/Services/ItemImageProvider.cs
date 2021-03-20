using DiscordBotHandler.Interfaces;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DiscordBotHandler.Services
{
    class ItemImageProvider : IStorageProvider<Image>
    {
        public Image GetObject(params object[] obj)
        {
            if (obj.Length < 1)
            {
                return null;
            }
            else
            {

                string itemName = obj[0].ToString();
                var itemImageUrl = @"http://cdn.dota2.com/apps/dota2/images/items/" + itemName.Replace("item_", "") + "_lg.png";
                var requestItem = WebRequest.Create(itemImageUrl);

                using (var responseItem = requestItem.GetResponse())
                using (var streamItem = responseItem.GetResponseStream())
                {
                    return Image.Load(streamItem);
                }
            }
        }
    }
}
