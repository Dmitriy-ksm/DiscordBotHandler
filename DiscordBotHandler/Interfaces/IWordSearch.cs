using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotHandler.Interfaces
{
    interface IWordSearch 
    {
        public string SearchWord(ulong guildId, string text);
        public void AddSearchWord(ulong guildId, string reply, params string[] word);
    }
}
