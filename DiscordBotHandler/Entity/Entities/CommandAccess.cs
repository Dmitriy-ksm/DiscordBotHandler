using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DiscordBotHandler.Entity.Entities
{
    public class CommandAccess
    {
        [Key]
        public string Command { get; set; }
        public virtual List<Channels> Channels { get; set; } = new List<Channels>();
    }
}
