using DiscordBotHandler.Interfaces;
using System.Threading.Tasks;

namespace DiscordBotHandler.Logger
{
    class LoggerEmpty : ILogger
    {
        public Task<Task> LogMessage(string message)
        {
            return Task.FromResult(Task.CompletedTask);
        }
    }
}
