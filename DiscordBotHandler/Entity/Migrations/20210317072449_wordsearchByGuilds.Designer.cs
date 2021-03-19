﻿// <auto-generated />
using System;
using DiscordBotHandler.Entity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscordBotHandler.Entity.Migrations
{
    [DbContext(typeof(EFContext))]
    [Migration("20210317072449_wordsearchByGuilds")]
    partial class wordsearchByGuilds
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
                            EthBtc = 0.03082,
                            EthBtcTime = "1615587672",
                            EthUsd = 1762.8,
                            EthUsdTime = "1615587672",
                            GasAvarage = 162
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
                        });
                });

            modelBuilder.Entity("DiscordBotHandler.Entity.Entities.WordSearch", b =>
                {
                    b.Property<string>("Reply")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Words")
                        .HasColumnType("TEXT");

                    b.HasKey("Reply");

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
#pragma warning restore 612, 618
        }
    }
}