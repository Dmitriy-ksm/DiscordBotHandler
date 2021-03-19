using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordBotHandler.Interfaces
{
    public interface IDraw 
    {
        public void DrawImage(object objects, MemoryStream stream);
    }
}
