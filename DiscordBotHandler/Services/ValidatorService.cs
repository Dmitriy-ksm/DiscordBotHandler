using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotHandler.Services
{
    public class ValidatorService : IValidator
    {
        private IVerificateCommand _verificator;
        public ValidatorService(IServiceProvider services)
        {
            _verificator = services.GetRequiredService<IVerificateCommand>();
        }
        public bool IsValid(string commandName, ulong guildId, ulong channelId, ILogger logger = null)
        {
            if (!_verificator.IsValid(commandName, guildId, channelId, out string debugString))
            {
                if(logger != null)
                    logger.LogMessage(debugString);

                return false;
            }
            return true;
        }
    }
}
