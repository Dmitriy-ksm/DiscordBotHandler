using Discord.Commands;
using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotHandler.Function.Modules.Player
{
    [Name("Player")]
    public class PlayerModule : ModuleBase<SocketCommandContext>
    {
        private readonly EFContext _db;
        private readonly IPlayer _player;
        private readonly IVerificateCommand _verificator;
        private readonly ILogger _logger;
        Dictionary<ulong, ulong> Channels;
        public PlayerModule(IServiceProvider services)
        {
            _player = services.GetRequiredService<IPlayer>();
            _verificator = services.GetRequiredService<IVerificateCommand>();
            _logger = services.GetRequiredService<ILogger>();
            _db = services.GetRequiredService<EFContext>();
            Channels = new Dictionary<ulong, ulong>();
            foreach (var item in _db.Guilds.ToList())
            {
                if (item.VoiceChannelId > 0)
                {
                    Channels.Add(item.GuildId, item.VoiceChannelId);
                }
            }

        }
        [Command("addVoiceChanel")]
        [Summary("Add voice channel for guild player")]
        public Task AddingVoiuceChannel([Summary("Voice channel id for the player")] ulong voiceChannelId)
        {
            if (_verificator.IsValid("player", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {
                var guildDb = _db.Guilds.FirstOrDefault(g => g.GuildId == Context.Guild.Id);
                if (guildDb == null)
                {
                    guildDb = new Entity.Entities.Guilds() { GuildId = Context.Guild.Id, VoiceChannelId = voiceChannelId };
                    _db.Guilds.Add(guildDb);
                }
                else
                {
                    guildDb.VoiceChannelId = voiceChannelId;
                }
                _db.SaveChanges();
                if (Channels.ContainsKey(Context.Guild.Id))
                {
                    Channels[Context.Guild.Id] = voiceChannelId;
                }
                else
                {
                    Channels.Add(Context.Guild.Id, voiceChannelId);
                }
            }
            else
            {
                _logger.LogMessage(debugString);
            }
            return Task.CompletedTask;
        }
        [Command("addTrack")]
        [Summary("Adding track to queue")]
        public Task AddingTrack([Summary("URL to track")] string uri)
        {
            if (_verificator.IsValid("player", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {
                _player.AddTrack(Context.Guild.Id, uri, 1);
            }
            else
            {
                _logger.LogMessage(debugString);
            }
            return Task.CompletedTask;
        }

        [Command("addTrackList")]
        [Summary("Adding track to queue")]
        public Task AddingTrackList([Summary("URL to tracks list")] string uri)
        {
            if (_verificator.IsValid("player", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {
                _player.AddTrack(Context.Guild.Id, uri, -1);
            }
            else
            {
                _logger.LogMessage(debugString);
            }
            return Task.CompletedTask;
        }

        [Command("play")]
        [Summary("Adding track to queue")]
        public Task Play()
        {
            if (Channels.ContainsKey(Context.Guild.Id))
            {
                ulong channel_id = Channels[Context.Guild.Id];
                if (_verificator.IsValid("player", Context.Guild.Id, Context.Channel.Id, out string debugString))
                {
                    _player.JoinChannel(Context.Guild.Id, channel_id);
                    _player.Start(Context.Guild.Id);
                }
                else
                {
                    _logger.LogMessage(debugString);
                }
            }
            else
            {
                _logger.LogMessage("Guild voice channel not set");
            }
            return Task.CompletedTask;
        }

        [Command("pause")]
        [Summary("Pausing track")]
        public Task Pause()
        {
            if (_verificator.IsValid("player", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {
                _player.Pause(Context.Guild.Id);
            }
            else
            {
                _logger.LogMessage(debugString);
            }
            return Task.CompletedTask;
        }
        [Command("resume")]
        [Summary("Resuming track")]
        public Task Resume()
        {
            if (_verificator.IsValid("player", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {
                _player.Resume(Context.Guild.Id);
            }
            else
            {
                _logger.LogMessage(debugString);
            }
            return Task.CompletedTask;
        }

        [Command("skip")]
        [Summary("Skiping track")]
        public Task Skip()
        {
            if (_verificator.IsValid("player", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {
                _player.Skip(Context.Guild.Id);
            }
            else
            {
                _logger.LogMessage(debugString);
            }
            return Task.CompletedTask;
        }

        [Command("stop")]
        [Summary("Stopimg track")]
        public Task Stop()
        {
            if (Channels.ContainsKey(Context.Guild.Id))
            {
                ulong channel_id = Channels[Context.Guild.Id];
                if (_verificator.IsValid("player", Context.Guild.Id, Context.Channel.Id, out string debugString))
                {
                    _player.Stop(Context.Guild.Id);
                    _player.LeaveChannel(Context.Guild.Id, channel_id);
                }
                else
                {
                    _logger.LogMessage(debugString);
                }
            }
            else
            {
                _logger.LogMessage("Guild voice channel not set");
            }
            return Task.CompletedTask;
        }
    }
}
