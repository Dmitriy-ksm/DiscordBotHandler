using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Entity.Entities;
using DiscordBotHandler.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBotHandler.Services
{
    class VerificateCommand : IVerificateCommand
    {
        private EFContext _db;
        public VerificateCommand(EFContext db)
        {
            _db = db;
        }

        public bool IsValid(string command, ulong guildId, ulong channelId, out string error)
        {
            bool isValid = false;
            error = "";
            var channelsForAll = _db.Channels.Include(c=>c.Commands).AsEnumerable().FirstOrDefault(c => c.GuildId== guildId && c.ChannelId == channelId && c.Commands.FirstOrDefault(com=>com.Command == "all")!=null);
            if(channelsForAll != null)
                isValid = true;
            else
            {
                var commandDb = _db.CommandAccesses.Include(c => c.Channels).FirstOrDefault(c => c.Command == command);
                if (commandDb == null)
                    error = "Команда не настроенна!";
                else if (commandDb.Channels.FirstOrDefault(c => c.ChannelId == channelId) == null)
                    error = "Команда не разрещенна для этого канала!";
                else
                    isValid = true;
            }
            return isValid;
        }

        public void SetPermit(string command, ulong guildId, ulong channelId)
        {
            var channelDb = CheckChannel(channelId, guildId, out _);
            CommandManage(command, channelDb);
            _db.SaveChanges();
        }

        public void UnsetPermit(string command, ulong guildId, ulong channelId)
        {
            var channelDb = CheckChannel(channelId, guildId, out bool hasChannel);
            if (hasChannel)
                CommandManage(command, channelDb, true);
            _db.SaveChanges();
        }

        private void CheckGuild(ulong guildId)
        {
            var guildDb = _db.Guilds.FirstOrDefault(g => g.GuildId == guildId);
            if (guildDb == null)
            {
                guildDb = new Guilds(){ GuildId = guildId };
                _db.Guilds.Add(guildDb);
            }
        }

        private Channels CheckChannel(ulong channelId, ulong guildId, out bool hasChannel)
        {
            CheckGuild(guildId);
            var channelDb = _db.Channels.FirstOrDefault(c => c.ChannelId == channelId);
            if (channelDb == null)
            {
                channelDb = new Channels()
                {
                    ChannelId = channelId,
                    GuildId = guildId,
                };
                _db.Channels.Add(channelDb);
                hasChannel = false;
            }
            hasChannel = true;
            return channelDb;
        }

        private void CommandManage(string command, Channels channel, bool IsRemove = false)
        {
            var commandDb = _db.CommandAccesses.Include(c => c.Channels).FirstOrDefault(c => c.Command == command);
            if (commandDb == null)
            {
                commandDb = new CommandAccess()
                {
                    Command = command,
                    Channels = new List<Channels>()
                };
                _db.CommandAccesses.Add(commandDb);
            }
            else
            {
                if(command == "all")
                {
                    channel.Commands = new List<CommandAccess>();
                    if (!IsRemove)
                    {
                        channel.Commands.Add(commandDb);
                    }
                }
                else
                {
                    if (commandDb.Channels.FirstOrDefault(c => c.ChannelId == channel.ChannelId) != null)
                    {
                        if (IsRemove)
                            commandDb.Channels.Remove(channel);
                    }
                    else
                        commandDb.Channels.Add(channel);
                }

                _db.CommandAccesses.Update(commandDb);
            }
        }
    }
}
