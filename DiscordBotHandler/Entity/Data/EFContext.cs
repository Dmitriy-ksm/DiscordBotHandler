using DiscordBotHandler.Entity.Entities;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DiscordBotHandler.Entity.Data
{
    public class EFContext : DbContext, IEFContext
    {


        public EFContext(DbContextOptions<EFContext> options) : base(options)
        {
            Database.Migrate();
            //Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddXmlFile("Connect.config");
            IConfigurationRoot config = builder.Build();
            string key = "key:attribute";
            options.UseSqlite(config[key], opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalMilliseconds));
        }


        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cooldown>().HasData(new Cooldown { Key = "wordsearch", KeyCooldown = 60, LastUse = DateTime.MinValue });
        }
        #endregion

        public DbSet<CryptoInfo> CryptoInfo { get; set; }
        public DbSet<CommandAccess> CommandAccesses { get; set; }
        public DbSet<Guilds> Guilds { get; set; }
        public DbSet<Channels> Channels { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<WordSearch> WordSearches { get; set; }

        public DbSet<Cooldown> Cooldowns { get; set; }
    }
}
