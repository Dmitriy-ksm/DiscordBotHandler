using System.IO;

namespace DiscordBotHandler.Interfaces
{
    public interface IDraw<T>
    {
        public void DrawImage(T objects, MemoryStream stream);
    }
}
