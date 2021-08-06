using System.ComponentModel.DataAnnotations;

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
