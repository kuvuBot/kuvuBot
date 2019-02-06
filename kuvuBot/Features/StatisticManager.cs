﻿using DSharpPlus;
using DSharpPlus.EventArgs;
using kuvuBot.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace kuvuBot.Features
{
    public class StatisticManager : IFeatureManager
    {
        public void Initialize(DiscordClient client)
        {
            client.GuildDownloadCompleted += Client_GuildDownloadCompleted;

            client.GuildCreated += Client_GuildCreated;
            client.GuildDeleted += Client_GuildDeleted;

            var aTimer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds); // elapse every 1 min
            int lastHour = DateTime.Now.Hour;
            aTimer.Elapsed += async (source, e) =>
            {
                if (lastHour < DateTime.Now.Hour || (lastHour == 23 && DateTime.Now.Hour == 0))
                {
                    lastHour = DateTime.Now.Hour;
                    // every hour
                    await Update();
                }
            };
        }

        public static int Guilds => Program.Client.Guilds.Count;
        public static int Channels => Program.Client.Guilds.Values.SelectMany(g => g.Channels).Count();
        public static int Users => Program.Client.Guilds.Values.SelectMany(g => g.Members).Count();
        static async Task Update()
        {

            var botContext = new BotContext();

            var last = botContext.Statistics.LastOrDefault();
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
                botContext.Statistics.Add(stat);
            }
            await botContext.SaveChangesAsync();
        }


        private static async Task Client_GuildDownloadCompleted(GuildDownloadCompletedEventArgs e)
        {
            await Update();
        }

        private static async Task Client_GuildDeleted(GuildDeleteEventArgs e)
        {
            await Update();
        }

        private static async Task Client_GuildCreated(GuildCreateEventArgs e)
        {
            await Update();
        }
    }
}
