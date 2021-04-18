using System.IO;

namespace DiscordBotHandler.Interfaces
{
    public interface IDraw 
    {
        public void DrawImage(object objects, MemoryStream stream);
    }
}
