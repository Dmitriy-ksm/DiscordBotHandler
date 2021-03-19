using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotHandler.Interfaces
{
     public interface IPlayer
    {
        public void JoinChannel(ulong guildId, ulong channel);
        public void LeaveChannel(ulong guildId, ulong channel);
        public void AddTrack(ulong guildid, string query, int maxCount);
        //public Task<Task> StartManagerAsync();
        public void Start(ulong guildId);
        public void Stop(ulong guildId);
        public void Pause(ulong guildId);
        public void Resume(ulong guildId);
        public void Skip(ulong guildId);
        public void SetVolume(ulong guildId, int volume);
        public void SeekTrack(ulong guildId, int seconds);
    }
}
