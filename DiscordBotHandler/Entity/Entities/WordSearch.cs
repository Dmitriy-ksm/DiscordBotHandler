using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiscordBotHandler.Entity.Entities
{
    public class WordSearch
    {
        [Key]
        public int Id { get; set; }
        public string Reply { get; set; }
        public string Words { get; set; }
    }
}
