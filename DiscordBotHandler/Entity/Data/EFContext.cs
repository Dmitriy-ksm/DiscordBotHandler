using DiscordBotHandler.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DiscordBotHandler.Entity.Data
{
    public class EFContext : DbContext
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

            modelBuilder.Entity<CryptoInfo>().HasData(new CryptoInfo
            { Id=1, EthUsdTime = "1615914281", EthUsd = 1800.65, GasAvarage = 227, EthBtc = 0.03233, EthBtcTime = "1615914281" });

            modelBuilder.Entity<UserInfo>().HasData(new UserInfo
            { Id = 228937227210719232, SteamId = 76561198064401017 },
            new UserInfo
            { Id = 409865565373726741, SteamId = 76561198017362452 },
            new UserInfo
            { Id = 516684301723762700, SteamId = 76561198160362424 },
            new UserInfo
            { Id = 723918995434111026, SteamId = 76561199066026629 });

            modelBuilder.Entity<Guilds>().HasData(new Guilds
            { GuildId = 715942326312108097, VoiceChannelId = 715951730856165417, WordSearches = new List<WordSearch>() });

           
        }
        #endregion

        public DbSet<CryptoInfo> CryptoInfo { get; set; }
        public DbSet<CommandAccess> CommandAccesses { get; set; }
        public DbSet<Guilds> Guilds { get; set; }
        public DbSet<Channels> Channels { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<WordSearch> WordSearches { get; set; }

    }
}
