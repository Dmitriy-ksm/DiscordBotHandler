using DiscordBotHandler.Interfaces;
using DiscordBotHandler.Services.Base;
using System;

namespace DiscordBotHandler.Services
{
    class ImageStorageItem : StorageProviderBase
    {
        public ImageStorageItem(IServiceProvider services) : base(services, StorageContains.DotaItem)
        {
        }
    }
}
