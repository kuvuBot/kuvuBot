using DSharpPlus;
using DSharpPlus.EventArgs;
using kuvuBot.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace kuvuBot
{
    public class StatisticManager
    {
        private static DiscordClient Client;
        public static void Initialize(DiscordClient client)
        {
            client.GuildDownloadCompleted += Client_GuildDownloadCompleted;

            client.GuildCreated += Client_GuildCreated;
            client.GuildDeleted += Client_GuildDeleted;

            //client.GuildMemberAdded += Client_GuildMemberAdded;
            //client.GuildMemberRemoved += Client_GuildMemberRemoved;
            Client = client;
        }

        async static Task Update(bool refreshGuilds = false, bool refreshChannels = false, bool refreshUsers = false)
        {
            int guilds = Client.Guilds.Count;
            int channels = Client.Guilds.Values.SelectMany(g => g.Channels).Count();
            int users = Client.Guilds.Values.SelectMany(g => g.Members).Count();

            var botContext = new BotContext();
            var stat = botContext.Statistics.FirstOrDefault(s => s.Equals(DateTime.Now));
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
            //else Disabled due to check stats every update
            //{
            //    if (refreshGuilds && stat.Guilds != guilds)
            //    {
            //        stat.Guilds = guilds;
            //    }
            //    if (refreshChannels && stat.Channels != channels)
            //    {
            //        stat.Channels = channels;
            //    }
            //    if (refreshUsers && stat.Users != users)
            //    {
            //        stat.Channels = channels;
            //    }
            //    if (refreshGuilds || refreshChannels || refreshUsers)
            //    {
            //        botContext.Update(stat);
            //    }
            //}
            await botContext.SaveChangesAsync();
        }


        // Init stats
        private async static Task Client_GuildDownloadCompleted(GuildDownloadCompletedEventArgs e)
        {
            await Update(true, true, true);
        }

        // Update guilds
        private static async Task Client_GuildDeleted(GuildDeleteEventArgs e)
        {
            await Update(true, true, true);
        }

        private static async Task Client_GuildCreated(GuildCreateEventArgs e)
        {
            await Update(true, true, true);
        }
    }
}
