using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace DiscordBotHandler.Entity.Data
{
    public class EFContextFactory: IDesignTimeDbContextFactory<EFContext>
    {
        public EFContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EFContext>();
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            //FileInfo file = new FileInfo("App.config");
            builder.AddXmlFile("Connect.config");
            IConfigurationRoot config = builder.Build();
            string key = "key:attribute";
            //Console.WriteLine(ConfigurationManager.ConnectionStrings["BotDB"].ConnectionString);
            optionsBuilder.UseSqlite(config[key], opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalMilliseconds));
            return new EFContext(optionsBuilder.Options);
        }
    }
}
