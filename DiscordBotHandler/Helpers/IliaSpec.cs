using Discord;
using Discord.WebSocket;
using DiscordBotHandler.Entity.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace DiscordBotHandler.Helpers
{
    public static class IliaSpec
    {
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
            if (client.ConnectionState == ConnectionState.Connected)
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

        public static void Init(out Timer timer,  DiscordSocketClient _client, EFContext _db)
        {
            timer = new Timer();
            timer.Interval = (1000 * 60 * 20 * 3); // 60 minutes...
            timer.Elapsed += ((object sender, ElapsedEventArgs e) => {
                SpecialIlia(Convert.ToUInt64((ConfigurationManager.GetSection("Ilia/user") as System.Collections.Hashtable)
               .Cast<System.Collections.DictionaryEntry>().ToDictionary(n => n.Key.ToString(), n => n.Value.ToString())["id"]),
               Convert.ToUInt64(ConfigurationManager.AppSettings["GuildId"]), _client, _db);
            });
            timer.AutoReset = true;
            timer.Start();
            SpecialIlia(Convert.ToUInt64((ConfigurationManager.GetSection("Ilia/user") as System.Collections.Hashtable)
                .Cast<System.Collections.DictionaryEntry>().ToDictionary(n => n.Key.ToString(), n => n.Value.ToString())["id"]),
                Convert.ToUInt64(ConfigurationManager.AppSettings["GuildId"]), _client, _db);
        }
    }
}
