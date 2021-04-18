using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DiscordBotHandler.Entity.Entities
{
    public class Guilds
    {
        [Key]
        public ulong GuildId { get; set; }
        public ulong VoiceChannelId { get; set; }
        public virtual List<WordSearch> WordSearches { get; set; }
    }
}
