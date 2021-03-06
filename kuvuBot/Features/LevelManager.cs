﻿using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using kuvuBot.Core.Features;
using kuvuBot.Data;

namespace kuvuBot.Features
{
    public class LevelManager : IFeature
    {
        public LevelManager(DiscordShardedClient client)
        {
            client.MessageCreated += Client_MessageCreated;
        }

        private static async Task Client_MessageCreated(MessageCreateEventArgs e)
        {
            if (e.Author.IsBot) return;
            await using var botContext = new BotContext();
            var kuvuGuild = await e.Guild.GetKuvuGuild(botContext);
            if (!e.Message.Content.StartsWith(kuvuGuild.Prefix))
            {
                var kuvuUser = await e.Author.GetKuvuUser(kuvuGuild, botContext);

                if (!kuvuUser.LastExpMessage.HasValue || (DateTime.Now - kuvuUser.LastExpMessage.Value).Minutes >= 1)
                {
                    if (kuvuGuild.ShowLevelUp)
                    {
                        await kuvuUser.AddExp(new Random().Next(1, 5), e.Channel, e.Author.Mention);
                    }
                    else
                    {
                        await kuvuUser.AddExp(new Random().Next(1, 5));
                    }

                    kuvuUser.LastExpMessage = DateTime.Now;

                    await botContext.SaveChangesAsync();
                }
            }
        }
    }
}
