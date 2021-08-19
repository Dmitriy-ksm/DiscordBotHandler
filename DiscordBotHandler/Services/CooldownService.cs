using DiscordBotHandler.Entity;
using DiscordBotHandler.Interfaces;
using System;
using System.Linq;

namespace DiscordBotHandler.Services
{
    public class CooldownService : ICooldown
    {
        private IEFContext _db;
        private ILogger _logger;
        public CooldownService(IEFContext db, ILogger logger)
        {
            _db = db;
            _logger = logger;
        }
        public bool Check(string key)
        {
            var time = DateTime.Now;
            var commandCooldown = _db.Cooldowns.FirstOrDefault(c => c.Key == key);
            if (commandCooldown == null || 
                time.Subtract(commandCooldown.LastUse).TotalSeconds > commandCooldown.KeyCooldown)
                return true;
            _logger.LogMessage($"{key} cooldown not expire yet");
            return false;
        }
        public void Set(string key)
        {
            var time = DateTime.Now;
            var commandCooldown = _db.Cooldowns.FirstOrDefault(c => c.Key == key);
            if (commandCooldown != null)
            {
                commandCooldown.LastUse = time;
                _db.Cooldowns.Update(commandCooldown);
                _db.SaveChanges();
            }
        }
    }
}
