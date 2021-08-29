using DiscordBotHandler.Entity;
using DiscordBotHandler.Entity.Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotHandler.Test.Classes
{
    public class FakeContext : DbContext, IEFContext
    {
        public virtual DbSet<CryptoInfo> CryptoInfo { get; set; }
        public virtual DbSet<CommandAccess> CommandAccesses { get; set; }
        public virtual DbSet<Guilds> Guilds { get; set; }
        public virtual DbSet<Channels> Channels { get; set; }
        public virtual DbSet<UserInfo> UserInfos { get; set; }
        public virtual DbSet<WordSearch> WordSearches { get; set; }
        public virtual DbSet<Cooldown> Cooldowns { get; set; }
    }
}
