using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using kuvuBot.Data;

namespace kuvuBot.Features
{
    class LevelManager : IFeatureManager
    {
        public void Initialize(DiscordClient client)
        {
            client.MessageCreated += Client_MessageCreated;
        }

        private static async Task Client_MessageCreated(MessageCreateEventArgs e)
        {
            if(e.Author.IsBot) return;
            using (var botContext = new BotContext())
            {
                var kuvuGuild = await e.Guild.GetKuvuGuild(botContext);
                if (!e.Message.Content.StartsWith(kuvuGuild.Prefix))
                {
                    var kuvuUser = await e.Author.GetKuvuUser(kuvuGuild, botContext);

                    if (!kuvuUser.LastExpMessage.HasValue || (DateTime.Now - kuvuUser.LastExpMessage.Value).Minutes >= 1)
                    {
                        await kuvuUser.AddExp(new Random().Next(1, 5), e.Channel, e.Author.Mention);
                        kuvuUser.LastExpMessage = DateTime.Now;

                        await botContext.SaveChangesAsync();
                    }
                }

            }
        }
    }
}
