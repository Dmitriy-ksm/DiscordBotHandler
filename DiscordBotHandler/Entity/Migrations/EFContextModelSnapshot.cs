﻿// <auto-generated />
using System;
using DiscordBotHandler.Entity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscordBotHandler.Entity.Migrations
{
    [DbContext(typeof(EFContext))]
    partial class EFContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.4");

            modelBuilder.Entity("ChannelsCommandAccess", b =>
                {
                    b.Property<ulong>("ChannelsChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CommandsCommand")
                        .HasColumnType("TEXT");

                    b.HasKey("ChannelsChannelId", "CommandsCommand");

                    b.HasIndex("CommandsCommand");

                    b.ToTable("ChannelsCommandAccess");
                });

            modelBuilder.Entity("DiscordBotHandler.Entity.Entities.Channels", b =>
                {
                    b.Property<ulong>("ChannelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ChannelId");

                    b.HasIndex("GuildId");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("DiscordBotHandler.Entity.Entities.CommandAccess", b =>
                {
                    b.Property<string>("Command")
                        .HasColumnType("TEXT");

                    b.HasKey("Command");

                    b.ToTable("CommandAccesses");
                });

            modelBuilder.Entity("DiscordBotHandler.Entity.Entities.Cooldown", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("KeyCooldown")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastUse")
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.ToTable("Cooldowns");

                    b.HasData(
                        new
                        {
                            Key = "wordsearch",
                            KeyCooldown = 60ul,
                            LastUse = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        });
                });

            modelBuilder.Entity("DiscordBotHandler.Entity.Entities.CryptoInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("EthBtc")
                        .HasColumnType("REAL");

                    b.Property<string>("EthBtcTime")
                        .HasColumnType("TEXT");

                    b.Property<double>("EthUsd")
                        .HasColumnType("REAL");

                    b.Property<string>("EthUsdTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("GasAvarage")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("CryptoInfo");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            EthBtc = 0.032329999999999998,
                            EthBtcTime = "1615914281",
                            EthUsd = 1800.6500000000001,
                            EthUsdTime = "1615914281",
                            GasAvarage = 227
                        });
                });

            modelBuilder.Entity("DiscordBotHandler.Entity.Entities.Guilds", b =>
                {
                    b.Property<ulong>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("VoiceChannelId")
                        .HasColumnType("INTEGER");

                    b.HasKey("GuildId");

                    b.ToTable("Guilds");

                    b.HasData(
                        new
                        {
                            GuildId = 715942326312108097ul,
                            VoiceChannelId = 715951730856165417ul
                        });
                });

            modelBuilder.Entity("DiscordBotHandler.Entity.Entities.UserInfo", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AdditionalInformationJSON")
                        .HasColumnType("TEXT");

                    b.Property<ulong?>("SteamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("UserInfos");

                    b.HasData(
                        new
                        {
                            Id = 228937227210719232ul,
                            SteamId = 76561198064401017ul
                        },
                        new
                        {
                            Id = 409865565373726741ul,
                            SteamId = 76561198017362452ul
                        },
                        new
                        {
                            Id = 516684301723762700ul,
                            SteamId = 76561198160362424ul
                        },
                        new
                        {
                            Id = 723918995434111026ul,
                            SteamId = 76561199066026629ul
                        });
                });

            modelBuilder.Entity("DiscordBotHandler.Entity.Entities.WordSearch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong?>("GuildsGuildId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Reply")
                        .HasColumnType("TEXT");

                    b.Property<string>("Words")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GuildsGuildId");

                    b.ToTable("WordSearches");
                });

            modelBuilder.Entity("ChannelsCommandAccess", b =>
                {
                    b.HasOne("DiscordBotHandler.Entity.Entities.Channels", null)
                        .WithMany()
                        .HasForeignKey("ChannelsChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DiscordBotHandler.Entity.Entities.CommandAccess", null)
                        .WithMany()
                        .HasForeignKey("CommandsCommand")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DiscordBotHandler.Entity.Entities.Channels", b =>
                {
                    b.HasOne("DiscordBotHandler.Entity.Entities.Guilds", "GuildOf")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuildOf");
                });

            modelBuilder.Entity("DiscordBotHandler.Entity.Entities.WordSearch", b =>
                {
                    b.HasOne("DiscordBotHandler.Entity.Entities.Guilds", null)
                        .WithMany("WordSearches")
                        .HasForeignKey("GuildsGuildId");
                });

            modelBuilder.Entity("DiscordBotHandler.Entity.Entities.Guilds", b =>
                {
                    b.Navigation("WordSearches");
                });
#pragma warning restore 612, 618
        }
    }
}
