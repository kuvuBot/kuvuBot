using DSharpPlus;
using DSharpPlus.EventArgs;
using kuvuBot.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DiscordBotsList.Api;
using kuvuBot.Core.Features;

namespace kuvuBot.Features
{
    public class StatisticManager : IFeature
    {
        private AuthDiscordBotListApi DiscordBotListApi { get; set; }

        public StatisticManager(DiscordShardedClient client)
        {
            client.GuildDownloadCompleted += Client_GuildDownloadCompleted;

            client.GuildCreated += Client_GuildCreated;
            client.GuildDeleted += Client_GuildDeleted;

            if (!string.IsNullOrEmpty(Program.Config.Apis.TopGg))
            {
                client.Ready += e =>
                {
                    DiscordBotListApi = new AuthDiscordBotListApi(client.CurrentUser.Id, Program.Config.Apis.TopGg);
                    return Task.CompletedTask;
                };
            }

            var aTimer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds); // elapse every 1 min
            var lastHour = DateTime.Now.Hour;
            aTimer.Elapsed += async (source, e) =>
            {
                if (lastHour < DateTime.Now.Hour || (lastHour == 23 && DateTime.Now.Hour == 0))
                {
                    lastHour = DateTime.Now.Hour;
                    // every hour
                    await Update();
                }
            };
            aTimer.Start();
        }

        public static int Guilds => Program.Client.ShardClients.Values.Sum(client => client.Guilds.Count);
        public static int Channels => Program.Client.ShardClients.Values.Sum(client => client.Guilds.Values.SelectMany(g => g.Channels).Count());
        public static int Users => Program.Client.ShardClients.Values.Sum(client => client.Guilds.Values.SelectMany(g => g.Members).Count());

        private async Task Update()
        {
            var botContext = new BotContext();

            var last = botContext.Statistics.ToList().LastOrDefault();
            if (last != null && last.Date.Date == DateTime.Now.Date && last.Guilds == Guilds && last.Channels == Channels && last.Users == Users)
                return;
            
            var stat = botContext.Statistics.FirstOrDefault(s => s.Date.Equals(DateTime.Now));
            if (stat == null)
            {
                stat = new KuvuStat
                {
                    Date = DateTime.Now,
                    Guilds = Guilds,
                    Channels = Channels,
                    Users = Users
                };
                
                Program.Client.DebugLogger.LogMessage(LogLevel.Info, nameof(StatisticManager), $"Saving statistics ({stat.Guilds} guilds, {stat.Channels} channels, {stat.Users})", DateTime.Now);
                await botContext.Statistics.AddAsync(stat);
                await botContext.SaveChangesAsync();

                if (DiscordBotListApi != null)
                {
                    try
                    {
                        await DiscordBotListApi.UpdateStats(stat.Guilds);
                    }
                    catch (Exception e)
                    {
                        Program.Client.DebugLogger.LogMessage(LogLevel.Error, nameof(StatisticManager), $"Failed sending stats to top.gg!\n{e}", DateTime.Now);
                    }
                }
            }
        }


        private async Task Client_GuildDownloadCompleted(GuildDownloadCompletedEventArgs e)
        {
            await Update();
        }

        private async Task Client_GuildDeleted(GuildDeleteEventArgs e)
        {
            await Update();
        }

        private async Task Client_GuildCreated(GuildCreateEventArgs e)
        {
            await Update();
        }
    }
}
