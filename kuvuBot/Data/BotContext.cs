using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
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
        public static async Task<KuvuUser> GetKuvuUser(this DiscordUser user, BotContext botContext = null)
        {
            botContext = botContext ?? new BotContext();

            var kuvuUser = await botContext.Users.FirstOrDefaultAsync(g => g.DiscordUser == user.Id);
            if (kuvuUser == null)
            {
                kuvuUser = new KuvuUser
                {
                    DiscordUser = user.Id,
                    Exp = 0,
                    Level = 1
                };
                botContext.Users.Add(kuvuUser);
                await botContext.SaveChangesAsync();
            }
            return kuvuUser;
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

        public ulong? MuteRole { get; set; }
    }

    public class KuvuStat
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int Guilds { get; set; }
        public int Channels { get; set; }
        public int Users { get; set; }
    }

    public class KuvuUser
    {
        public int Id { get; set; }

        public ulong DiscordUser { get; set; }
        public uint Level { get; set; }
        public uint Exp { get; set; }

        public virtual List<KuvuWarn> Warns { get; set; }
    }


    public class KuvuWarn
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public ulong Warning { get; set; }
        public virtual KuvuUser User { get; set; }
        public virtual KuvuGuild Guild { get; set; }
        public int Weight { get; set; }
        public string Reason { get; set; }
    }

    public class BotContext : DbContext
    {
        public DbSet<KuvuGuild> Guilds { get; set; }
        public DbSet<KuvuStat> Statistics { get; set; }
        public DbSet<KuvuUser> Users { get; set; }
        public DbSet<KuvuWarn> Warns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            var mysqlconfig = Program.LoadConfig().MySQL;
            string connectionString = $"SERVER={mysqlconfig.Ip};DATABASE={mysqlconfig.Database};UID={mysqlconfig.User};PASSWORD={mysqlconfig.Password};PORT={mysqlconfig.Port}";

            optionsBuilder.UseMySql(connectionString, mysqlOptions =>
            {

            });
        }
    }
}
