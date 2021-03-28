using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotHandler.Interfaces
{
    public interface ICooldown
    {
        public bool Check(string key);
        public void Set(string key);
    }
}
