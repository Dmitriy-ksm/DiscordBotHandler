using DiscordBotHandler.Interfaces;
using System;
using System.Threading.Tasks;

namespace DiscordBotHandler.Logger
{
    class LoggerConsole : ILogger
    {
        public Task<Task> LogMessage(string message)
        {
            Console.WriteLine(message);
            return Task.FromResult(Task.CompletedTask);
        }
    }
}
