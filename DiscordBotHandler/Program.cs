using System;
using System.Threading.Tasks;
using Discord;
using System.Configuration;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using DiscordBotHandler.Function;
using Discord.Commands;
using System.Net.Http;
using DiscordBotHandler.Services;
using DiscordBotHandler.Entity.Data;
using System.Reflection;
using DiscordBotHandler.Interfaces;
using System.Text.Json;
using System.Timers;
using Image = SixLabors.ImageSharp.Image;

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
            var lavalinkManager = services.GetService<IPlayer>();
            var _db = services.GetService<EFContext>();

            var commandHandler = services.GetService<CommandHandler>();
            System.Timers.Timer t1 = new System.Timers.Timer();
            t1.Interval = (1000 * 60 * 20 * 3); // 60 minutes...
            t1.Elapsed += ((object sender, ElapsedEventArgs e)=>{
                SpecialIlia(Convert.ToUInt64((ConfigurationManager.GetSection("Ilia/user") as System.Collections.Hashtable)
               .Cast<System.Collections.DictionaryEntry>().ToDictionary(n => n.Key.ToString(), n => n.Value.ToString())["id"]),
               Convert.ToUInt64(ConfigurationManager.AppSettings["GuildId"]), _client, _db);
            });
            t1.AutoReset = true;
            t1.Start();
            SpecialIlia(Convert.ToUInt64((ConfigurationManager.GetSection("Ilia/user") as System.Collections.Hashtable)
                .Cast<System.Collections.DictionaryEntry>().ToDictionary(n => n.Key.ToString(), n => n.Value.ToString())["id"]),
                Convert.ToUInt64(ConfigurationManager.AppSettings["GuildId"]), _client, _db);

            await _client.LoginAsync(TokenType.Bot,
                    ConfigurationManager.AppSettings["BotToken"]);
            await _client.StartAsync();

            await commandHandler.InitializeAsync();
            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
        private ServiceProvider ConfigureServices()
        {

            return new ServiceCollection()
                .AddDbContext<EFContext>()
                .AddSingleton<DiscordSocketClient>()
                .AddTransient<ItemImageProvider>()
                .AddTransient<HeroImageProvider>()
                .AddSingleton<ImageStorageHero>()
                .AddSingleton<ImageStorageItem>()
                .AddTransient<Func<StorageContains, IStorageProvider<Image>>>(serviceProvider => key =>
                {
                    switch (key)
                    {
                        case StorageContains.DotaHero:
                            return serviceProvider.GetService<HeroImageProvider>();
                        case StorageContains.DotaItem:
                            return serviceProvider.GetService<ItemImageProvider>();
                        default:
                            throw new KeyNotFoundException();
                    }
                })
                .AddTransient<Func<StorageContains, IStorage<Image>>>(serviceProvider => key =>
                {
                    switch (key)
                    {
                        case StorageContains.DotaHero:
                            return serviceProvider.GetService<ImageStorageHero>();
                        case StorageContains.DotaItem:
                            return serviceProvider.GetService<ImageStorageItem>();
                        default:
                            throw new KeyNotFoundException();
                    }
                })
                .AddSingleton<HttpClient>()
                .AddSingleton<ILogger, Logger.Logger>()
                .AddSingleton<IDraw, DotaImageDraw>()
                .AddSingleton<IDotaAssistans, DotaAssistans>()
                .AddSingleton<ICrypto, Crypto>()
                .AddSingleton<IPlayer, Player>()
                .AddSingleton<IWordSearch,WordSearchService>()
                .AddSingleton<IVerificateCommand,VerificateCommand>()
                 .AddSingleton((provider) =>
                 {
                     var service = new CommandService();
                     service.AddModulesAsync(Assembly.GetEntryAssembly(), provider)
                         .GetAwaiter().GetResult();
                     return service;
                 })
                 .AddSingleton<ICooldown,Cooldown>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }

        public static void SpecialIlia(ulong iliaId, ulong guildId, Discord.WebSocket.DiscordSocketClient client, EFContext db)
        {

            var ilyaxaDb = db.UserInfos.FirstOrDefault(u => u.Id == iliaId);
            Dictionary<string, string> nickData = (ilyaxaDb != null && ilyaxaDb.AdditionalInformationJSON != null) ?
                JsonSerializer.Deserialize<Dictionary<string, string>>(ilyaxaDb.AdditionalInformationJSON) :
                new Dictionary<string, string>();
            var ilyaxaNickName = nickData.ContainsKey("nick") ? nickData["nick"].Split('.', StringSplitOptions.RemoveEmptyEntries) : new string[1] { "Empty" };

            int today = (DateTime.Now - new DateTime(DateTime.Now.Year, 1, 1)).Days + 1;
            int index = today % ilyaxaNickName.Length;
            string new_ilya_nick = ilyaxaNickName[index];
            if(client.ConnectionState == ConnectionState.Connected)
            {
                var server = client.GetGuild(guildId);
                var user = server.GetUser(iliaId);
                Task.Run(async () =>
                {
                    await user.ModifyAsync(x =>
                        {
                            x.Nickname = new_ilya_nick;
                        });
                });
            }
            else
            {
                client.Ready += async () =>
                {
                    var server = client.GetGuild(guildId);
                    var user = server.GetUser(iliaId);
                    await user.ModifyAsync(x =>
                    {
                        x.Nickname = new_ilya_nick;
                    });
                };
            }
        }
    }
}
