using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Entity.Entities;
using DiscordBotHandler.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBotHandler.Services
{
    class WordSearchService : IWordSearch
    {
        private readonly EFContext _db;
        public WordSearchService(IServiceProvider services)
        {
            _db = services.GetRequiredService<EFContext>();
        }
        public string SearchWord(ulong guildId, string text)
        {
            var wSbGidDb = _db.Guilds.Include(w => w.WordSearches).AsEnumerable().FirstOrDefault(w => w.GuildId == guildId);
            if(wSbGidDb != null)
            {
                foreach (var item in wSbGidDb.WordSearches)
                {
                    string[] search = item.Words.Split("/", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in search)
                    {
                        if (text.ToLower().StartsWith(word.ToLower()))
                        {
                            return item.Reply;
                        }
                    }
                }
            }
            return null;
        }
        public void AddSearchWord(ulong guildId, string reply, params string[] search)
        {
            var wordSearchesByGuildsDb = _db.Guilds.Include(w => w.WordSearches).AsEnumerable().FirstOrDefault(w => w.GuildId == guildId);
            if(wordSearchesByGuildsDb == null)
            {
                var searchDb = new WordSearch()
                {
                    Reply = reply,
                    Words = string.Join(" ", search)
                };
                wordSearchesByGuildsDb = new Guilds() 
                { 
                    GuildId = guildId,
                    WordSearches = new List<WordSearch>() { searchDb }
                };
                _db.Guilds.Add(wordSearchesByGuildsDb);
            }
            else
            {
                var searchDb = wordSearchesByGuildsDb.WordSearches.FirstOrDefault(w => w.Reply == reply);
                if(searchDb == null)
                {
                    searchDb = new WordSearch()
                    {
                        Reply = reply,
                        Words = string.Join(" ", search)
                    };
                    wordSearchesByGuildsDb.WordSearches.Add(searchDb);
                }
                else
                {
                    searchDb.Words = string.Join(" ", search);
                    _db.WordSearches.Update(searchDb);
                }
            }
            _db.SaveChanges();
        }
    }
}
