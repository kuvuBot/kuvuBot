using DSharpPlus;
using DSharpPlus.EventArgs;
using kuvuBot.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace kuvuBot.Commands
{
    public class LogController
    {
        private static DiscordClient Client;
        public static void Initialize(DiscordClient client)
        {
            client.GuildMemberAdded += Client_GuildMemberAdded;
            client.GuildMemberRemoved += Client_GuildMemberRemoved;

            Client = client;
        }

        private async static Task Client_GuildMemberRemoved(GuildMemberRemoveEventArgs e)
        {
            var kuvuGuild = await e.Guild.GetKuvuGuild();
        }

        private async static Task Client_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            var kuvuGuild = await e.Guild.GetKuvuGuild();
            
        }
    }
}
