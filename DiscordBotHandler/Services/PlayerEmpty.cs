using Discord.WebSocket;
using DiscordBotHandler.Interfaces;

namespace DiscordBotHandler.Services
{
    class PlayerEmpty : IPlayer
    {
        public PlayerEmpty(DiscordSocketClient client, ILogger logger)
        {

        }
       
        public void JoinChannel(ulong guildId, ulong channel_id)
        {
            
        }

        public void LeaveChannel(ulong guildId, ulong channel_id)
        {

        }

        public void AddTrack(ulong guildId, string query, int maxCount)
        {
            
        }
        
        public void Start(ulong guildId)
        {  
        }

        public void Skip(ulong guildId)
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
