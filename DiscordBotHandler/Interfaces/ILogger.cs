using System.Threading.Tasks;

namespace DiscordBotHandler.Interfaces
{
    public interface ILogger
    {
        Task<Task> LogMessage(string message);
    }
}
