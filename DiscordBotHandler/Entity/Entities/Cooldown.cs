using System;
using System.ComponentModel.DataAnnotations;

namespace DiscordBotHandler.Entity.Entities
{
    public class Cooldown
    {
        [Key]
        public string Key { get; set; }
        public ulong KeyCooldown { get; set; }
        public DateTime LastUse { get; set; }
    }
}
