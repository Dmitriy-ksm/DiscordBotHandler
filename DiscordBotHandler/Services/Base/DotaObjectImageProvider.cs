using DiscordBotHandler.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
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
                Image recipeItemImage = null;
                string objName = obj[0].ToString();
                if (objName.Contains("recipe"))
                {
                    string recipeItem = objName.Replace("recipe_", "");
                    string urlItem = url = @"https://cdn.cloudflare.steamstatic.com/apps/dota2/images//items/" + recipeItem.Replace("item_", "") + "_lg.png";
                    var requestItemRecipe = WebRequest.Create(url);
                    using (var responseItem = requestItemRecipe.GetResponse())
                    {
                        using (var streamItem = responseItem.GetResponseStream())
                        {
                            recipeItemImage = Image.Load(streamItem);
                            recipeItemImage.Mutate(i => i.Resize(new Size((int)(recipeItemImage.Width * 0.75), (int)(recipeItemImage.Height * 0.75))));
                        }
                    }
                    objName = "recipe";
                    //TODO прикрутить картинку самого предмета в углу рецепта
                }
                if (_type.Equals(StorageContains.DotaHero))
                    url = @"https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/" + objName.Replace("npc_dota_hero_", "") + ".png";
                if(_type.Equals(StorageContains.DotaItem))
                    url = @"https://cdn.cloudflare.steamstatic.com/apps/dota2/images//items/" + objName.Replace("item_", "") + "_lg.png";

                var requestItem = WebRequest.Create(url);
                using (var responseItem = requestItem.GetResponse())
                {
                    using (var streamItem = responseItem.GetResponseStream())
                    {
                        Image result = Image.Load(streamItem);
                        if(recipeItemImage != null)
                        {
                            result.Mutate(i => i.DrawImage(recipeItemImage,new Point((int)(result.Width*0.25),(int)(result.Height*0.25)), 1f));
                        }
                        return result;
                    }
                }
            }
        }
    }
}
