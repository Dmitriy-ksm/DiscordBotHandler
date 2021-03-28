using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
