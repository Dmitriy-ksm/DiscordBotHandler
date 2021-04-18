using Discord.Commands;
using DiscordBotHandler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBotHandler.Function.Modules.WordSearch
{
    [Name("wordsearch")]
    public class WordSearchModule : ModuleBase<SocketCommandContext>
    {
        private readonly IVerificateCommand _verificator;
        private readonly IWordSearch _wordSearch;
        private readonly ILogger _logger;
        private static Dictionary<ulong,string> replyCache { get; set; } = new Dictionary<ulong, string>();
        private static Dictionary<ulong, string> wordCache { get; set; } = new Dictionary<ulong, string>();
        public WordSearchModule(IServiceProvider services)
        {
            _verificator = services.GetRequiredService<IVerificateCommand>();
            _wordSearch = services.GetRequiredService<IWordSearch>();
            _logger = services.GetRequiredService<ILogger>();
        }
        [Command("addWordsAndReply")]
        [Summary("Adding words and replies")]
        [RequireBotModerationRole]
        public Task AddWordsAndReply([Summary("One word reply")] string reply, [Summary("Many words for replied, split them by '/'")] params string[] words)
        {
            if (_verificator.IsValid("wordsearch", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {
                _wordSearch.AddSearchWord(Context.Guild.Id, reply, words);
            }
            else
            {
                _logger.LogMessage(debugString);
            }
            return Task.CompletedTask;
        }
        [Command("addReplyCache")]
        [Summary("Adding replies cache")]
        [RequireBotModerationRole]
        public Task AddReplyCache([Summary("Many words reply")]  params string[] reply)
        {
            if (_verificator.IsValid("wordsearch", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {
                if (replyCache.ContainsKey(Context.Guild.Id))
                {
                    replyCache[Context.Guild.Id] = string.Join(' ', reply);
                }
                else
                {
                    replyCache.Add(Context.Guild.Id, string.Join(' ', reply));
                }
            }
            else
            {
                _logger.LogMessage(debugString);
            }
            return Task.CompletedTask;
        }
        [Command("addWordCache")]
        [Summary("Adding words cache")]
        [RequireBotModerationRole]
        public Task AddWordCache([Summary("Many words for replied, split them by '/'")]params string[] words)
        {
            if (_verificator.IsValid("wordsearch", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {
                if (wordCache.ContainsKey(Context.Guild.Id))
                {
                    wordCache[Context.Guild.Id] = string.Join(' ', words);
                }
                else
                {
                    wordCache.Add(Context.Guild.Id, string.Join(' ', words));
                }
            }
            else
            {
                _logger.LogMessage(debugString);
            }
            return Task.CompletedTask;
        }
        [Command("aplyCache")]
        [Summary("Aply cache")]
        [RequireBotModerationRole]
        public Task AplyCache()
        {
            if (_verificator.IsValid("wordsearch", Context.Guild.Id, Context.Channel.Id, out string debugString))
            {
                if (wordCache.ContainsKey(Context.Guild.Id) && wordCache[Context.Guild.Id] != null &&
                    replyCache.ContainsKey(Context.Guild.Id) && replyCache[Context.Guild.Id] != null)
                {
                    _wordSearch.AddSearchWord(Context.Guild.Id, replyCache[Context.Guild.Id], wordCache[Context.Guild.Id]);
                    replyCache[Context.Guild.Id] = null;
                    wordCache[Context.Guild.Id] = null;
                }
            }
            else
            {
                _logger.LogMessage(debugString);
            }
            return Task.CompletedTask;
        }
    }
}
