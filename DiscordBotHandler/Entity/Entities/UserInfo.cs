using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiscordBotHandler.Entity.Entities
{
    public class UserInfo
    {
        [Key]
        public ulong Id { get; set; }
        public ulong? SteamId { get; set; }
        public string AdditionalInformationJSON {get;set;}
    }
}
