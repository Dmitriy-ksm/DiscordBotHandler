using DiscordBotHandler.Interfaces;
using System.Threading.Tasks;

namespace DiscordBotHandler.Logger
{
    class LoggerEmpty : ILogger
    {
        public LoggerEmpty()
        {

        }
        public async Task<Task> LogMessage(string message)
        {
            return Task.CompletedTask;
        }
    }
}
