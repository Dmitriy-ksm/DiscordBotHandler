using DiscordBotHandler.Interfaces;
using System.Threading.Tasks;

namespace DiscordBotHandler.Services
{
    public class LoggerEmptyService : ILogger
    {
        public Task<Task> LogMessage(string message)
        {
            return Task.FromResult(Task.CompletedTask);
        }
    }
}
