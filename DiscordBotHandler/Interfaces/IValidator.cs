using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotHandler.Interfaces
{
    interface IValidator
    {
        bool IsValid(string commandName, ulong guildId, ulong channelId, ILogger logger);
    }
}
