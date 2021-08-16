using DiscordBotHandler.Interfaces;
using DiscordBotHandler.Services.Base;
using System;

namespace DiscordBotHandler.Services
{
    public class ImageStorageHeroService : StorageProviderBase
    {
        public ImageStorageHeroService(IServiceProvider services) : base(services, StorageContains.DotaHero)
        {
        }
    }
}
