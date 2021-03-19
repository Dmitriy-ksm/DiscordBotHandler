using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotHandler.Interfaces
{
    public interface ILogger
    {
        Task<Task> LogMessage(string message);
    }
}
