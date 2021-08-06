using Discord.WebSocket;
using DiscordBotHandler.Interfaces;

namespace DiscordBotHandler.Services
{
    class PlayerEmpty : IPlayer
    {
        public PlayerEmpty(DiscordSocketClient client, ILogger logger)
        {

        }
       
        public async void JoinChannel(ulong guildId, ulong channel_id)
        {
            
        }

        public async void LeaveChannel(ulong guildId, ulong channel_id)
        {
            
        }

        public async void AddTrack(ulong guildId, string query, int maxCount)
        {
            
        }
        
        public void Start(ulong guildId)
        {  
        }

        public async void Skip(ulong guildId)
        {
           
        }

        public void Stop(ulong guildId)
        {
            
        }

        public void Pause(ulong guildId)
        {
           
        }

        public void Resume(ulong guildId)
        {
            
        }

        public void SetVolume(ulong guildId, int volume)
        {
            
        }
        public void SeekTrack(ulong guildId, int seconds)
        {
            
        }
    }
}
