using DiscordBotHandler.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotHandler.Entity
{
    public interface IEFContext 
    {
        DbSet<CryptoInfo> CryptoInfo { get; set; }
        DbSet<CommandAccess> CommandAccesses { get; set; }
        DbSet<Guilds> Guilds { get; set; }
        DbSet<Channels> Channels { get; set; }
        DbSet<UserInfo> UserInfos { get; set; }
        DbSet<WordSearch> WordSearches { get; set; }
        DbSet<Cooldown> Cooldowns { get; set; }

        int SaveChanges();
        //Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
