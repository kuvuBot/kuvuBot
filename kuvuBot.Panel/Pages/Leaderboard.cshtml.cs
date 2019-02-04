using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using DSharpPlus;
using DSharpPlus.Net;
using kuvuBot.Data;
using DSharpPlus.Entities;

namespace kuvuBot.Panel.Pages
{
    public class LeaderboardModel : PageModel
    {
        public KuvuGuild Guild { get; set; }
        public DiscordGuild DGuild { get; set; }
        public DiscordRestClient Client { get; set; }
        public BotContext _BotContext { get; set; }

        public async Task<ActionResult> OnGetAsync(string search)
        {
            _BotContext = new BotContext();
            if (!string.IsNullOrWhiteSpace(search))
            {
                if (ulong.TryParse(search, out var guildId))
                {
                    Guild = _BotContext.Guilds.FirstOrDefault(x => x.GuildId == guildId);
                }
                else
                {
                    Guild = _BotContext.Guilds.OrderBy(x => Commands.CommandUtils.LevenshteinDistance(kuvuBot.Program.Client.GetGuildAsync(x.GuildId).Result.Name, search)).FirstOrDefault();
                }
                if (Guild != null)
                    DGuild = await kuvuBot.Program.Client.GetGuildAsync(Guild.GuildId);
                if (Guild == null || DGuild == null)
                {
                    TempData["message"] = $"Error: {search} was not found";
                    return RedirectToPage("Leaderboard", new { });
                }
            }
            else if (User.Identity.IsAuthenticated)
            {
                var token = await HttpContext.GetTokenAsync("access_token");

                Client = new DiscordRestClient(new DiscordConfiguration
                {
                    TokenType = TokenType.Bearer,
                    Token = token,
                });
                await Client.InitializeCacheAsync();
            }
            return Page();
        }
    }
}