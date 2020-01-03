using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using kuvuBot.Lang;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Steam.Models;

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

        public static async Task<GlobalUser> GetGlobalUser(this DiscordUser user, BotContext botContext = null)
        {
            if (user.IsBot) throw new Exception("Can't get bot GlobalUser");
            botContext = botContext ?? new BotContext();

            var globalUser = await botContext.GlobalUsers.FirstOrDefaultAsync(g => g.DiscordUser == user.Id);
            if (globalUser == null)
            {
                globalUser = new GlobalUser
                {
                    DiscordUser = user.Id,
                    Money = 0,
                    Reputation = 0,
                    GlobalRank = null
                };
                botContext.GlobalUsers.Add(globalUser);
                await botContext.SaveChangesAsync();
            }
            return globalUser;
        }

        public static async Task<KuvuUser> GetKuvuUser(this DiscordUser user, KuvuGuild guild, BotContext botContext = null)
        {
            if (user.IsBot) throw new Exception("Can't get bot kuvuUser");
            botContext = botContext ?? new BotContext();

            var kuvuUser = await botContext.Users.FirstOrDefaultAsync(g => g.DiscordUser == user.Id && g.Guild == guild);
            if (kuvuUser == null)
            {
                kuvuUser = new KuvuUser
                {
                    DiscordUser = user.Id,
                    Exp = 0,
                    Guild = guild
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

        public bool ShowLevelUp { get; set; } = false;

        public ulong? GreetingChannelId { get; set; }

        [NotMapped]
        public DiscordChannel GreetingChannel
        {
            get
            {
                if (!GreetingChannelId.HasValue)
                    return null;

                return Program.Client.GetGuildAsync(GuildId).GetAwaiter().GetResult().GetChannel(GreetingChannelId.Value);
            }
            set => GreetingChannelId = value?.Id;
        }
        public string GreetingMessage { get; set; }

        public ulong? GoodbyeChannelId { get; set; }

        [NotMapped]
        public DiscordChannel GoodbyeChannel
        {
            get
            {
                if (!GoodbyeChannelId.HasValue)
                    return null;

                return Program.Client.GetGuildAsync(GuildId).GetAwaiter().GetResult().GetChannel(GoodbyeChannelId.Value);
            }
            set => GoodbyeChannelId = value?.Id;
        }
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

    public enum KuvuGlobalRank { Helper, Moderator, Admin, Root }
    public class GlobalUser
    {
        public int Id { get; set; }

        public ulong DiscordUser { get; set; }
        public KuvuGlobalRank? GlobalRank { get; set; }
        public int Reputation { get; set; } = 0;
        public DateTime? LastGivedRep { get; set; }
        public int Money { get; set; } = 0;

        public virtual List<KuvuUser> KuvuUsers { get; set; }
    }

    public class KuvuUser
    {
        public int Id { get; set; }

        public ulong DiscordUser { get; set; }
        public int Exp { get; set; }
        public DateTime? LastExpMessage { get; set; }

        // GameDevAlgorithms.com 
        public const float LevelModifier = 0.5f;
        public static int ConvertExpToLevel(int exp)
        {
            return (int)(LevelModifier * MathF.Sqrt(exp));
        }

        public static int ConvertLevelToExp(int level)
        {
            // XP = (Level / 0.05) ^ 2
            return (int)Math.Pow(level / LevelModifier, 2);
        }

        public int GetLevel()
        {
            return ConvertExpToLevel(Exp);
        }

        public async Task AddExp(int exp, DiscordChannel channel = null, string mention = null)
        {
            var currentLevel = GetLevel();
            Exp += exp;
            var nextLevel = GetLevel();
            if (nextLevel > currentLevel)
            {
                if (channel != null && mention != null)
                    await channel.SendMessageAsync((await channel.Guild.Lang("level.promotion")).Replace("{mention}", mention).Replace("{level}", nextLevel.ToString()));
            }
        }

        public virtual List<KuvuWarn> Warns { get; set; }
        public virtual KuvuGuild Guild { get; set; }
        public virtual GlobalUser GlobalUser { get; set; }
    }


    public class KuvuWarn
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public ulong Warning { get; set; }
        public virtual KuvuUser User { get; set; }
        public int Weight { get; set; }
        public string Reason { get; set; }
    }

    public class KuvuLog
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Application { get; set; }
        public string Message { get; set; }
    }

    public enum CacheType { Steam }
    public class CacheInfo
    {
        [Key]
        public CacheType Type { get; set; }

        public DateTime RefreshedTime { get; set; }
    }

    public class BotContext : DbContext
    {
        public DbSet<KuvuGuild> Guilds { get; set; }
        public DbSet<KuvuStat> Statistics { get; set; }
        public DbSet<KuvuUser> Users { get; set; }
        public DbSet<KuvuWarn> Warns { get; set; }
        public DbSet<KuvuLog> Logs { get; set; }

        public DbSet<GlobalUser> GlobalUsers { get; set; }

        public DbSet<SteamAppModel> SteamAppsCache { get; set; }
        public DbSet<CacheInfo> CacheInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            var mysqlconfig = Program.LoadConfig().MySQL;
            string connectionString = $"SERVER={mysqlconfig.Ip};DATABASE={mysqlconfig.Database};UID={mysqlconfig.User};PASSWORD={mysqlconfig.Password};PORT={mysqlconfig.Port}";

            optionsBuilder.UseMySql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SteamAppModel>()
                .HasKey(a => a.AppId);
            modelBuilder.Entity<CacheInfo>()
                .Property(e => e.Type)
                .HasConversion(new EnumToStringConverter<CacheType>());
            modelBuilder.Entity<GlobalUser>()
                .Property(e => e.GlobalRank)
                .HasConversion(new EnumToStringConverter<KuvuGlobalRank>());
        }
    }
}
