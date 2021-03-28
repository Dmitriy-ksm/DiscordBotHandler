using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            if (commandCooldown == null || time.Subtract(commandCooldown.LastUse).TotalSeconds > commandCooldown.KeyCooldown)
            {
                return true;
            }
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
