using DSharpPlus;
using DSharpPlus.EventArgs;
using kuvuBot.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace kuvuBot.Features
{
    public class StatisticManager : IFeatureManager
    {
        public void Initialize(DiscordClient client)
        {
            client.GuildDownloadCompleted += Client_GuildDownloadCompleted;

            client.GuildCreated += Client_GuildCreated;
            client.GuildDeleted += Client_GuildDeleted;
        }

        static async Task Update(DiscordClient client)
        {
            int guilds = client.Guilds.Count;
            int channels = client.Guilds.Values.SelectMany(g => g.Channels).Count();
            int users = client.Guilds.Values.SelectMany(g => g.Members).Count();

            var botContext = new BotContext();
            var stat = botContext.Statistics.FirstOrDefault(s => s.Date.Equals(DateTime.Now));
            if (stat == null)
            {
                stat = new KuvuStat()
                {
                    Date = DateTime.Now,
                    Guilds = guilds,
                    Channels = channels,
                    Users = users
                };
                botContext.Statistics.Add(stat);
            }
            await botContext.SaveChangesAsync();
        }


        private static async Task Client_GuildDownloadCompleted(GuildDownloadCompletedEventArgs e)
        {
            await Update(e.Client);
        }

        private static async Task Client_GuildDeleted(GuildDeleteEventArgs e)
        {
            await Update(e.Client);
        }

        private static async Task Client_GuildCreated(GuildCreateEventArgs e)
        {
            await Update(e.Client);
        }
    }
}
