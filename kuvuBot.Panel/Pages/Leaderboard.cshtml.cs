using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using kuvuBot.Panel.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using DSharpPlus;
using DSharpPlus.Net;

namespace kuvuBot.Panel.Pages
{
    public class LeaderboardModel : PageModel
    {
        public ulong? GuildId { get; set; }
        public DiscordRestClient Client { get; set; }

        public async Task OnGetAsync(ulong? guildId)
        {
            GuildId = guildId;

            if (!guildId.HasValue && User.Identity.IsAuthenticated)
            {
                var token = await HttpContext.GetTokenAsync("access_token");

                Client = new DiscordRestClient(new DiscordConfiguration
                {
                    TokenType = TokenType.Bearer,
                    Token = token,
                });
                await Client.InitializeCacheAsync();
            }
        }
    }
}