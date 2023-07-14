using System;
using System.Threading.Tasks;
using Discord;
using System.Configuration;
using Discord.WebSocket;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using DiscordBotHandler.Function;
using Discord.Commands;
using System.Net.Http;
using DiscordBotHandler.Services;
using DiscordBotHandler.Entity.Data;
using System.Reflection;
using DiscordBotHandler.Interfaces;
using Image = SixLabors.ImageSharp.Image;
using DiscordBotHandler.Helpers;
using DiscordBotHandler.Services.Providers;
using DiscordBotHandler.Helpers.Dota;
using DiscordBotHandler.Entity;

namespace DiscordBotHandler
{
    class Program
    {
        private DiscordSocketClient _client;
        public static void Main(string[] args)
                => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var services = ConfigureServices();
            _client = services.GetService<DiscordSocketClient>();
            var _db = services.GetService<EFContext>();

            var commandHandler = services.GetService<CommandHandler>();
            IliaSpec.Init(out _, _client, _db);

            await _client.LoginAsync(TokenType.Bot,
                    ConfigurationManager.AppSettings["BotToken"]);
            await _client.StartAsync();

            await commandHandler.InitializeAsync();
            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
        private ServiceProvider ConfigureServices()
        {
            var config = new DiscordSocketConfig()
                {
                    GatewayIntents = GatewayIntents.All
                };
            return new ServiceCollection()
                .AddDbContext<EFContext>()
                .AddScoped<IEFContext>(provider => provider.GetService<EFContext>())
                .AddSingleton<DiscordSocketClient>(serviceProvider => new DiscordSocketClient(config))
                .AddSingleton<ImageStorageHeroService>()
                .AddSingleton<ImageStorageItemService>()
                .AddTransient<Func<StorageContains, IStorageProvider<Image>>>(serviceProvider => key =>
                {
                    switch (key)
                    {
                        case StorageContains.DotaHero:
                            return new DotaObjectImageProvider(StorageContains.DotaHero);
                        case StorageContains.DotaItem:
                            return new DotaObjectImageProvider(StorageContains.DotaItem);
                        default:
                            throw new KeyNotFoundException();
                    }
                })
                .AddTransient<Func<StorageContains, IStorage<Image>>>(serviceProvider => key =>
                {
                    switch (key)
                    {
                        case StorageContains.DotaHero:
                            return serviceProvider.GetService<ImageStorageHeroService>();
                        case StorageContains.DotaItem:
                            return serviceProvider.GetService<ImageStorageItemService>();
                        default:
                            throw new KeyNotFoundException();
                    }
                })
                .AddSingleton<HttpClient>()
                .AddSingleton<ILogger, LoggerConsoleService>()
                .AddSingleton<IDraw<DotaGameResult>, DotaImageDrawService>()
                .AddSingleton<IDotaAssistans, DotaAssistansService>()
                .AddSingleton<ICrypto, CryptoService>()
                .AddSingleton<IPlayer, PlayerEmptyService>()
                .AddSingleton<IWordSearch, WordSearchService>()
                .AddSingleton<IVerificateCommand, VerificateCommandService>()
                .AddSingleton<IValidator, ValidatorService>()
                 .AddSingleton((provider) =>
                 {
                     var service = new CommandService();
                     service.AddModulesAsync(Assembly.GetEntryAssembly(), provider)
                         .GetAwaiter().GetResult();
                     return service;
                 })
                 .AddSingleton<ICooldown, CooldownService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }
    }
}
