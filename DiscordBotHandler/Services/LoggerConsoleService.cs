using DiscordBotHandler.Interfaces;
using System;
using System.Threading.Tasks;

namespace DiscordBotHandler.Services
{
    public class LoggerConsoleService : ILogger
    {
        public Task<Task> LogMessage(string message)
        {
            Console.WriteLine(message);
            return Task.FromResult(Task.CompletedTask);
        }
    }
}
