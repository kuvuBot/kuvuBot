using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

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
            var kuvuUser
        }
    }
}
