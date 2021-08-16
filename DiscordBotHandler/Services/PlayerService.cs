using Discord.WebSocket;
using DiscordBotHandler.Interfaces;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Events;
using Lavalink4NET.Logging;
using Lavalink4NET.Player;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotHandler.Services
{
    public class PlayerService : IPlayer
    {
        private readonly DiscordSocketClient _client;
        private readonly LavalinkNode lavalinkManager;
        private Dictionary<ulong, Queue<LavalinkTrack>> _tracks;
        //Not tested yet
        public Dictionary<ulong, Queue<LavalinkTrack>> Tracks =>
            _tracks ??= new Dictionary<ulong, Queue<LavalinkTrack>>();
        public Interfaces.ILogger _logger;
        LavalinkWrapperLogger _loggerLvl2;
        public PlayerService(DiscordSocketClient client, Interfaces.ILogger logger)
        {
            _client = client;
            _logger = logger;
            _loggerLvl2 = new LavalinkWrapperLogger();
            _loggerLvl2.SetLogger(_logger);
            lavalinkManager = new LavalinkNode(new LavalinkNodeOptions
            {
                RestUri = "http://localhost:2333/",
                WebSocketUri = "ws://localhost:2333/",
                Password = ConfigurationManager.AppSettings["LavalinkPassword"],
                //DebugPayloads = true,
                DisconnectOnStop = false,
            }, new DiscordClientWrapper(client), _loggerLvl2);
            _client.Ready += () => lavalinkManager.InitializeAsync();
            lavalinkManager.TrackEnd += OnTrackEnd;
        }
        //Not tested yet
        private bool CheckExistGuildTrackQueryAndAddNewIfNone(ulong guildId)
        {
            if (!Tracks.ContainsKey(guildId))
            {
                Tracks.Add(guildId, new Queue<LavalinkTrack>());
                return false;
            }
            return true;
        }
        private async Task<Task> OnTrackEnd(object sender, TrackEndEventArgs eventArgs)
        {
            if (eventArgs.Reason == TrackEndReason.Finished)
            {
                if (Tracks[eventArgs.Player.GuildId].Count > 0)
                    await eventArgs.Player.PlayAsync(Tracks[eventArgs.Player.GuildId].Dequeue());
            }
            return Task.CompletedTask;
        }


        public async void JoinChannel(ulong guildId, ulong channel_id)
        {
            _ = lavalinkManager.GetPlayer<LavalinkPlayer>(guildId) ?? await lavalinkManager.JoinAsync(guildId, channel_id);
        }

        public async void LeaveChannel(ulong guildId, ulong channel_id)
        {
            var player = lavalinkManager.GetPlayer<LavalinkPlayer>(guildId);
            if (player != null)
                await player.DisconnectAsync();

            if (CheckExistGuildTrackQueryAndAddNewIfNone(guildId))
                Tracks[guildId] = new Queue<LavalinkTrack>();

        }

        public async void AddTrack(ulong guildId, string query, int maxCount)
        {
            CheckExistGuildTrackQueryAndAddNewIfNone(guildId);

            var response = await lavalinkManager.GetTracksAsync(query);
            if (response != null)
            {
                if (maxCount > 0)
                {
                    int j = 0;
                    LavalinkTrack[] responseTrackArray = response.ToArray();
                    for (int i = maxCount; i > 0; i--)
                    {
                        if (j + 1 > response.Count())
                            break;

                        Tracks[guildId].Enqueue(responseTrackArray[j++]);
                    }
                }
                else
                {
                    foreach (var searchedTracks in response)
                        Tracks[guildId].Enqueue(searchedTracks);
                }
            }
        }

        public void Start(ulong guildId)
        {
            var player = lavalinkManager.GetPlayer<LavalinkPlayer>(guildId);
            if (player != null && !(player.State == PlayerState.Playing))
            {
                if (CheckExistGuildTrackQueryAndAddNewIfNone(guildId))
                {
                    if (Tracks[guildId].Count > 0)
                    {
                        player.PlayAsync(Tracks[guildId].Dequeue());
                    }
                }
            }

        }

        public async void Skip(ulong guildId)
        {
            var player = lavalinkManager.GetPlayer<LavalinkPlayer>(guildId);
            if (player != null && (player.State == PlayerState.Playing))
            {
                await player.StopAsync();
                if (CheckExistGuildTrackQueryAndAddNewIfNone(guildId))
                {
                    if (Tracks[guildId].Count > 0)
                    {
                        _ = player.PlayAsync(Tracks[guildId].Dequeue());
                    }
                }
            }
        }

        public void Stop(ulong guildId)
        {
            var player = lavalinkManager.GetPlayer<LavalinkPlayer>(guildId);
            if (player != null && (player.State == PlayerState.Playing))
                player.StopAsync();

            if (CheckExistGuildTrackQueryAndAddNewIfNone(guildId))
            {
                Tracks[guildId] = new Queue<LavalinkTrack>();
            }
        }

        public void Pause(ulong guildId)
        {
            var player = lavalinkManager.GetPlayer<LavalinkPlayer>(guildId);
            if (player != null && (player.State == PlayerState.Playing))
                player.PauseAsync();
        }

        public void Resume(ulong guildId)
        {
            var player = lavalinkManager.GetPlayer<LavalinkPlayer>(guildId);
            if (player != null && !(player.State == PlayerState.Playing) && Tracks.Count > 0)
                player.ResumeAsync();
        }

        public void SetVolume(ulong guildId, int volume)
        {
            var player = lavalinkManager.GetPlayer<LavalinkPlayer>(guildId);
            float volumeF = volume / 100;
            if (player != null)
                player.SetVolumeAsync(volumeF);
        }
        public void SeekTrack(ulong guildId, int seconds)
        {
            var player = lavalinkManager.GetPlayer<LavalinkPlayer>(guildId);
            if (player != null && !(player.State == PlayerState.Playing))
                player.SeekPositionAsync(TimeSpan.FromSeconds(seconds));
        }
    }
    class LavalinkWrapperLogger : Lavalink4NET.Logging.ILogger
    {
        public Interfaces.ILogger logger;
        public void SetLogger(Interfaces.ILogger logger)
        {
            this.logger = logger;
        }
        void Lavalink4NET.Logging.ILogger.Log(object source, string message, LogLevel level, Exception exception)
        {
            logger.LogMessage(message + Environment.NewLine + (exception?.Message ?? ""));
        }
    }
}
