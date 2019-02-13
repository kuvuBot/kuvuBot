using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using kuvuBot.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kuvuBot.Panel.Pages.Panel.Guild
{
    [Authorize]
    public class BaseGuildModel : PageModel
    {
        public DiscordRestClient Client;
        public BotContext _BotContext;

        public bool Global = false;
        public GlobalUser globalUser;

        public KuvuGuild Guild;
        public DiscordGuild DGuild;

        public virtual async Task<ActionResult> OnGetAsync(string id)
        {
            _BotContext = new BotContext();
            Client = await HttpContext.GetRestClient();

            if (ulong.TryParse(id, out var guildId))
            {
                Guild = _BotContext.Guilds.FirstOrDefault(x => x.GuildId == guildId);
                if (Guild != null)
                    DGuild = Client.Guilds.Values.FirstOrDefault(x => x.Id == Guild.GuildId);
                if (Guild == null || DGuild == null || !DGuild.Permissions.HasValue || !DGuild.Permissions.Value.HasPermission(Permissions.ManageGuild))
                {
                    return Unauthorized();
                }
            }
            else if (id == "global")
            {
                globalUser = await Client.CurrentUser.GetGlobalUser(_BotContext);
                if (globalUser.GlobalRank >= KuvuGlobalRank.Admin)
                {
                    Global = true;
                }
                else
                {
                    return Unauthorized();
                }
            }

            return Page();
        }
    }
}
