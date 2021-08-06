using DiscordBotHandler.Interfaces;
using DiscordBotHandler.Services.Base;
using System;

namespace DiscordBotHandler.Services
{
    class ImageStorageHero : StorageProviderBase
    {
        public ImageStorageHero(IServiceProvider services) : base(services, StorageContains.DotaHero)
        {
        }
    }
}
