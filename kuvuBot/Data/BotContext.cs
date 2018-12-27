using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace kuvuBot.Data
{
    public static class BotContextExt
    {
        public static async Task<KuvuGuild> GetKuvuGuild(this DiscordGuild guild, BotContext botContext = null)
        {
            botContext = botContext ?? new BotContext();

            var kuvuGuild = await botContext.Guilds.FirstOrDefaultAsync(g => g.GuildId == guild.Id);
            if (kuvuGuild == null)
            {
                kuvuGuild = new KuvuGuild()
                {
                    GuildId = guild.Id,
                    Lang = "en",
                    Prefix = Program.Config.DefualtPrefix,
                };
                botContext.Guilds.Add(kuvuGuild);
                await botContext.SaveChangesAsync();
            }
            return kuvuGuild;
        }
    }

    public class KuvuGuild
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }

        public string Lang { get; set; }
        public string Prefix { get; set; }
        public ulong? LogChannel { get; set; }
        public ulong? AutoRole { get; set; }

        public ulong? GreetingChannel { get; set; }
        public string GreetingMessage { get; set; }
        public ulong? GoodbyeChannel { get; set; }
        public string GoodbyeMessage { get; set; }
    }

    public class KuvuStat
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int Guilds { get; set; }
        public int Channels { get; set; }
        public int Users { get; set; }
    }

    public class BotContext : DbContext
    {
        public DbSet<KuvuGuild> Guilds { get; set; }
        public DbSet<KuvuStat> Statistics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var mysqlconfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json")).MySQL;
            string connectionString = $"SERVER={mysqlconfig.Ip};DATABASE={mysqlconfig.Database};UID={mysqlconfig.User};PASSWORD={mysqlconfig.Password};persistsecurityinfo=True;port={mysqlconfig.Port};SslMode=none";

            optionsBuilder.UseMySql(connectionString, mysqlOptions =>
            {

            });
        }
    }
}
