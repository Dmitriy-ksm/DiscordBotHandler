using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotHandler.Interfaces
{
    public interface IVerificateCommand
    {
        bool IsValid(string command, ulong guildId, ulong channelId, out string error);
        void SetPermit(string command, ulong guildId, ulong channelId);
        void UnsetPermit(string command, ulong guildId, ulong channelId);
    }
}
