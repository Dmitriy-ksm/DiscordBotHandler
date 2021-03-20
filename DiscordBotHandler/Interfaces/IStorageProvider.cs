using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotHandler.Interfaces
{
    public interface IStorageProvider<out T>
    {
        public T GetObject(params object[] obj);
    }
}
