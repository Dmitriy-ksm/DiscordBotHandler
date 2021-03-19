using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiscordBotHandler.Entity.Entities
{
    public class Channels
    {
        [Key]
        public ulong ChannelId { get; set; }
        [ForeignKey("GuildOf")]
        public ulong GuildId { get; set; } 
        public virtual Guilds GuildOf { get; set; }
        public List<CommandAccess> Commands { get; set; } = new List<CommandAccess>();
    }
}
