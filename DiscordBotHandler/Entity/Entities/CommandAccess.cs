using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DiscordBotHandler.Entity.Entities
{
    public class CommandAccess
    {
        [Key]
        public string Command { get; set; }
        public virtual List<Channels> Channels { get; set; } = new List<Channels>();
    }
}
