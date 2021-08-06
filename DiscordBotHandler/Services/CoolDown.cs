using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Interfaces;
using System;
using System.Linq;

namespace DiscordBotHandler.Services
{
    class Cooldown : ICooldown
    {
        private EFContext _db;
        private ILogger _logger;
        public Cooldown(EFContext db, ILogger logger)
        {
            _db = db;
            _logger = logger;
        }
        public bool Check(string key)
        {
            var time = DateTime.Now;
            var commandCooldown = _db.Cooldowns.FirstOrDefault(c => c.Key == key);
            if(commandCooldown == null) 
            {
                return true;
            }
            if(time.Subtract(commandCooldown.LastUse).TotalSeconds > commandCooldown.KeyCooldown)
            {
                commandCooldown.LastUse = time;
                _db.Cooldowns.Update(commandCooldown);
                _db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
