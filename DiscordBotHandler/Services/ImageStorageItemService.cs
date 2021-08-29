using DiscordBotHandler.Interfaces;
using DiscordBotHandler.Services.Base;
using System;

namespace DiscordBotHandler.Services
{
    public class ImageStorageItemService : StorageProviderBase
    {
        public ImageStorageItemService(IServiceProvider services) : base(services, StorageContains.DotaItem)
        {
        }
    }
}
