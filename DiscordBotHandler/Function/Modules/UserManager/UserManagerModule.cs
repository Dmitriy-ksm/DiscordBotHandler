using Discord.Commands;
using Discord.WebSocket;
using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Entity.Entities;
using DiscordBotHandler.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordBotHandler.Function.Modules.UserManager
{
    [Name("UserManager")]
    public class UserManagerModule : ModuleBase<SocketCommandContext>
    {
        private readonly IVerificateCommand _verificator;
        private readonly EFContext _db;
        private readonly IDotaAssistans _dota;
        private readonly ILogger _logger;
        public UserManagerModule(IServiceProvider services)
        {
            _db = services.GetRequiredService<EFContext>();
            _verificator = services.GetRequiredService<IVerificateCommand>();
            _dota = services.GetRequiredService<IDotaAssistans>();
            _logger = services.GetRequiredService<ILogger>();
        }
        [Command("addInfoes")]
        [Summary("Adding additional info for users")]
        [RequireBotModerationRole]
        public Task SetAdditionalInfoes(SocketUser user, string key, params string[] valueFull)
        {
            string value = string.Join(" ", valueFull);
            if (_verificator.IsValid("usermanager", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {
                var userDb = _db.UserInfos.FirstOrDefault(u => u.Id == user.Id);
                if (userDb != null)
                {
                    Dictionary<string, string> info = userDb.AdditionalInformationJSON == null ? 
                        new Dictionary<string,string>() : 
                        JsonSerializer.Deserialize<Dictionary<string, string>>(userDb.AdditionalInformationJSON);
                    if (info == null)
                    {
                        info = new Dictionary<string, string>();
                    }
                    if (info.ContainsKey(key))
                    {
                        info[key] = value;
                    }
                    else
                    {
                        info.Add(key, value);
                    }
                    userDb.AdditionalInformationJSON = JsonSerializer.Serialize(info);
                    _db.UserInfos.Update(userDb);
                }
                else
                {
                    userDb = new UserInfo()
                    {
                        Id = user.Id,
                        AdditionalInformationJSON = JsonSerializer.Serialize(new Dictionary<string, string>() { { key, value } })
                    };
                    _db.UserInfos.Add(userDb);
                }
                _db.SaveChanges();
            }
            else
            {
                _logger.LogMessage(debugString);
            }
            return Task.CompletedTask;
        }
        [Command("addSteamUser")]
        [Summary("Adding steamId to discord user")]
        [RequireBotModerationRole]
        public Task SetSteamId(SocketUser user, string url)
        {
            if (_verificator.IsValid("usermanager", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {
                ulong steamId = Task.Run(async () => { return await _dota.GetSteamIdAsync(url); }).Result;
                if (steamId == 0)
                {
                    return ReplyAsync("SteamId не найден");
                }
                var userDb = _db.UserInfos.FirstOrDefault(u => u.Id == user.Id);
                if (userDb != null)
                {
                    userDb.SteamId = steamId;
                }
                else
                {
                    _db.UserInfos.Add(new UserInfo() { Id = user.Id, SteamId = steamId });
                }
                _db.SaveChanges();
                ReplyAsync("Пользователь сохраннен");
            }
            else
            {
                _logger.LogMessage(debugString);
            }
            return Task.CompletedTask;
        }
    }
}
