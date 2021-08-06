namespace DiscordBotHandler.Interfaces
{
    interface IWordSearch 
    {
        public string SearchWord(ulong guildId, string text);
        public void AddSearchWord(ulong guildId, string reply, params string[] word);
    }
}
