using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DSharpPlus;
using kuvuBot.Data;
using DSharpPlus.Entities;
using kuvuBot.Core.Commands;

namespace kuvuBot.Panel.Pages
{
    public class LeaderboardModel : PageModel
    {
        public KuvuGuild Guild { get; set; }
        public DiscordGuild DGuild { get; set; }
        public DiscordRestClient Client { get; set; }
        public BotContext _BotContext { get; set; }

        public async Task<ActionResult> OnGetAsync(ulong? id)
        {
            _BotContext = new BotContext();
            if (id.HasValue)
            {
                Guild = _BotContext.Guilds.FirstOrDefault(x => x.GuildId == id);

                if (Guild != null)
                {
                    try
                    {
                        DGuild = await kuvuBot.Program.Client.GetGuildAsync(Guild.GuildId);
                    }
                    catch (Exception)
                    {
                        TempData["message"] = $"Error: {id} cannot be retrieved, ensure bot is still there";
                        return Redirect("~/Leaderboard");
                        throw;
                    }
                }

                if (Guild == null || DGuild == null)
                {
                    TempData["message"] = $"Error: {id} was not found";
                    return Redirect("~/Leaderboard");
                }
            }
            else if (User.Identity.IsAuthenticated)
            {
                Client = await HttpContext.GetRestClient();
            }

            return Page();
        }
    }
}